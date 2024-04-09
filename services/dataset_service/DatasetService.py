import pandas as pd
from typing import List
from pydantic import BaseModel
from sklearn.preprocessing import StandardScaler


class DatasetSettings(BaseModel):
    path: str
    savePath: str
    isNormalize: bool
    isLoadDst: bool
    isLoadKp: bool
    isLoadAp: bool
    isLoadWolf: bool
    dropna: bool
    timeColumn: str = ''
    columns: List[str] = []


class DatasetService:
    def get_columns(self, path):
        df = pd.read_csv(path)
        return df.columns.tolist()

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

    def load_wolf(self, df, time_column):
        path = 'D:\\CommercialProjects\\Infragrad.Worker\\Infrastructure\\Mlsat.Sync\\Wolf.csv'
        wolf = pd.read_csv(path)
        wolf = wolf.rename(columns={'Date': time_column})
        self.add_space_weather(df, wolf, 'wolf', time_column)

    def load_ap(self, df, time_column):
        path = 'D:\\CommercialProjects\\Infragrad.Worker\\Infrastructure\\Mlsat.Sync\\Ap.csv'
        ap = pd.read_csv(path)
        ap = ap.rename(columns={'Date': time_column})
        self.add_space_weather(df, ap, 'ap', time_column)

    def load_dst(self, df, time_column):
        path = 'D:\\CommercialProjects\\Infragrad.Worker\\Infrastructure\\Mlsat.Sync\\Dst.csv'
        dst = pd.read_csv(path)
        dst = ap.rename(columns={'Date': time_column})
        self.add_space_weather(df, dst, 'dst', time_column)

    def load_kp(self, df, time_column):
        path = 'D:\\CommercialProjects\\Infragrad.Worker\\Infrastructure\\Mlsat.Sync\\Kp.csv'
        kp = pd.read_csv(path)
        kp = ap.rename(columns={'Date': time_column})
        self.add_space_weather(df, kp, 'kp', time_column)

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

        if hasTimeColumn and settings.isLoadWolf:
            self.load_wolf(df, settings.timeColumn)

        if hasTimeColumn and settings.isLoadAp:
            self.load_ap(df, settings.timeColumn)

        if hasTimeColumn and settings.isLoadKp:
            self.load_kp(df, settings.timeColumn)

        if hasTimeColumn and settings.isLoadDst:
            self.load_dst(df, settings.timeColumn)

        df.to_csv(savePath)

        return settings
