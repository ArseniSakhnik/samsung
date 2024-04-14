import pandas as pd
from pydantic import BaseModel
from typing import List
import numpy as np
from sklearn.neighbors import NearestNeighbors
from services.plot_service.PlotService import PlotService
from services.dataset_service.DatasetService import DatasetService


class KnnSettings(BaseModel):
    datasetPath: str
    savePath: str
    timeColumn: str
    nNeighbors: int = 5
    algorithm: str
    percentile: int = 95
    spaceWeatherColumns: List[str] = []
    columns: List[str] = []


class KnnService:
    def __init__(self) -> None:
        self.plot_service = PlotService()
        self.data_service = DatasetService()
        super().__init__()

    def prepareData(self, data, column, time_column, space_wather_columns):
        columns = []
        columns.append(column)
        if len(time_column) != 0:
            columns.append(time_column)

        for item in space_wather_columns:
            columns.append(item)

        df = data[columns]
        df['index'] = df.reset_index().index + 1
        df.rename(columns={time_column: 'timestamp'}, inplace=True)
        df['timestamp'] = pd.to_datetime(df['timestamp'])
        df['time_epoch'] = (df['timestamp'].astype(np.int64) / 100000000000).astype(np.int64)
        return df

    def knn(self, df, column, settings: KnnSettings):
        x = df[[column]]
        # модель
        knn_model = NearestNeighbors(n_neighbors=settings.nNeighbors, algorithm=settings.algorithm)
        knn_model.fit(x)
        distances, _ = knn_model.kneighbors(x)
        k_distance = distances[:, -1]
        threshold = np.percentile(k_distance, settings.percentile)
        # модель
        anomalies_indices = np.where(k_distance > threshold)[0]
        df[f'anomaly_{column}'] = 0
        df.loc[anomalies_indices, f'anomaly_{column}'] = 1

        if len(settings.spaceWeatherColumns) != 0:
            self.plot_service.show_corr_matrix(df,
                                               f'anomaly_{column}',
                                               settings.spaceWeatherColumns,
                                               column,
                                               settings.savePath)

        self.plot_service.print_scutter_only(df, anomalies_indices, column, settings.savePath)
        self.plot_service.print_plot(df, anomalies_indices, column, settings.savePath)
        for space_weather in settings.spaceWeatherColumns:
            self.plot_service.print_plot(df, anomalies_indices, column, settings.savePath, space_weather)

        self.data_service.save_csv(df, column, settings.savePath)

    def learn(self, settings: KnnSettings):
        data = pd.read_csv(settings.datasetPath)
        self.data_service.minimal_processing(data, settings.timeColumn)
        for column in settings.columns:
            df = self.prepareData(data, column, settings.timeColumn, settings.spaceWeatherColumns)
            self.knn(df.head(5000), column, settings)
