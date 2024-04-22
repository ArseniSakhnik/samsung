import pandas as pd
from pydantic import BaseModel
from typing import List
import numpy as np
from sklearn.neighbors import LocalOutlierFactor
from services.plot_service.PlotService import PlotService
from services.dataset_service.DatasetService import DatasetService

class LofSettings(BaseModel):
    datasetPath: str
    savePath: str
    timeColumn: str
    spaceWeatherColumns: List[str] = []
    columns: List[str] = []
    # гиперпараметры
    nNeighbors: int
    contamination: float

class LofService:
    def __init__(self) -> None:
        self.plot_service = PlotService()
        self.data_service = DatasetService()
        super().__init__()

    def lof(self, df, column, settings: LofSettings):
        x = df[[column]]

        # модель
        lof_model = LocalOutlierFactor(n_neighbors=settings.nNeighbors, contamination=settings.contamination)  # Параметры можно настраивать
        y_pred = lof_model.fit_predict(x)
        anomalies_indices = np.where(y_pred == -1)[0]
        #модель
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

    def learn(self, settings: LofSettings):
        data = pd.read_csv(settings.datasetPath)
        self.data_service.minimal_processing(data, settings.timeColumn)
        for column in settings.columns:
            df = self.data_service.prepareData(data, column, settings.timeColumn, settings.spaceWeatherColumns)
            self.lof(df, column, settings)
