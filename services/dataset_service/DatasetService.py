import pandas as pd
from typing import List, Dict, Optional
from datetime import datetime
from pydantic import BaseModel
from sklearn.preprocessing import StandardScaler
import os

base_directory = './python/space__weathers'


class SpaceWeatherPoint(BaseModel):
    points: Dict[datetime, float]


class DatasetSettings(BaseModel):
    path: str
    savePath: str
    isNormalize: bool
    dropna: bool
    timeColumn: str = ''
    columns: List[str] = [],
    dst: Optional[Dict[datetime, float]] = None
    kp: Optional[Dict[datetime, float]] = None
    ap: Optional[Dict[datetime, float]] = None
    wolf: Optional[Dict[datetime, float]] = None


class DatasetService:
    def create_folder_if_not_exist(self, path):
        os.makedirs(path, exist_ok=True)

    def get_columns(self, path):
        df = pd.read_csv(path)
        return df.columns.tolist()

    def get_dates(self, path, time_column):
        df = pd.read_csv(path)
        self.convert_values(df, time_column)
        return {
            "startDate": df[time_column].min(),
            "endDate": df[time_column].max()
        }

    def convert_values(self, df, time_column):
        df[time_column] = pd.to_datetime(df[time_column]).dt.tz_localize(None)
        df = df.sort_values(by=time_column)
        df.set_index(time_column, inplace=True)

    def add_space_weather(self, data, df, w, time_column):
        self.convert_values(df, time_column)
        data[w] = None
        for index, row in data.iterrows():
            nearest_row = df.iloc[(df[time_column] - row[time_column]).abs().argsort()[:1]]
            nearest_value = nearest_row['Value'].values[0] if not nearest_row.empty else None
            data.at[index, w] = nearest_value

    def get_points(self, points: Dict[datetime, float], time_column):
        response = {
            f'{time_column}': [],
            'Value': []
        }

        for key, value in points.items():
            response[f'{time_column}'].append(key)
            response['Value'].append(value)

        df = pd.DataFrame(response)

        return df

    def load_wolf(self, df, time_column, points: Dict[datetime, float]):
        wolf = self.get_points(points, time_column)
        self.add_space_weather(df, wolf, 'wolf', time_column)

    def load_ap(self, df, time_column, points: Dict[datetime, float]):
        ap = self.get_points(points, time_column)
        self.add_space_weather(df, ap, 'ap', time_column)

    def load_dst(self, df, time_column, points: Dict[datetime, float]):
        dst = self.get_points(points, time_column)
        self.add_space_weather(df, dst, 'dst', time_column)

    def load_kp(self, df, time_column, points: Dict[datetime, float]):
        kp = self.get_points(points, time_column)
        self.add_space_weather(df, kp, 'kp', time_column)

    def minimal_processing(self, df, time_column):
        column_types = df.dtypes
        drop_columns = []

        for column, dtype in column_types.items():
            if column == time_column:
                continue
            if dtype == 'object':
                drop_columns.append(column)
                continue

            df[column] = df[column].astype(float)

        if len(drop_columns) != 0:
            df = df.drop(columns=[drop_columns])

        df.fillna(0, inplace=True)

    def save_csv(self, df, column, save_path):
        directory_path = f'{save_path}\\{column}'
        self.create_folder_if_not_exist(directory_path)
        path = f'{directory_path}\\{column}_anomaly.csv'
        df.to_csv(path)


    def create_on_basis(self, settings: DatasetSettings):
        scaler = StandardScaler()
        hasTimeColumn = len(settings.timeColumn) > 0
        base_data = pd.read_csv(settings.path)

        df = base_data[settings.columns]

        if hasTimeColumn:
            df[settings.timeColumn] = base_data[settings.timeColumn]

        column_types = df.dtypes

        for column, dtype in column_types.items():
            if dtype == 'object' and column == settings.timeColumn:
                df[column] = pd.to_datetime(df[column])
                df[column] = df[column].dt.strftime('%Y-%m-%d %H:%M:%S')
                continue
            elif dtype == 'object':
                df = df.drop(columns=[column])
                continue
            elif dtype == 'int64':
                df[column] = df[column].astype(float)
            if settings.isNormalize:
                df[column] = scaler.fit_transform(df[[column]])

        if settings.dropna:
            df.dropna(axis=1, how='any', inplace=True)
        else:
            df.fillna(0, inplace=True)

        if hasTimeColumn:
            self.convert_values(df, settings.timeColumn)

        if hasTimeColumn and settings.wolf != None:
            self.load_wolf(df, settings.timeColumn, settings.wolf)

        if hasTimeColumn and settings.ap != None:
            self.load_ap(df, settings.timeColumn, settings.ap)

        if hasTimeColumn and settings.kp != None:
            self.load_kp(df, settings.timeColumn, settings.kp)

        if hasTimeColumn and settings.dst != None:
            self.load_dst(df, settings.timeColumn, settings.dst)

        df.to_csv(settings.savePath)

        return settings
