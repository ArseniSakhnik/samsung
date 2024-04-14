import pandas as pd
from pydantic import BaseModel
from typing import List
import numpy as np
from sklearn.ensemble import IsolationForest
from services.plot_service.PlotService import PlotService
from services.dataset_service.DatasetService import DatasetService

class IsloationForestSettings(BaseModel):
    datasetPath: str
    savePath: str
    timeColumn: str
    spaceWeatherColumns: List[str] = []
    columns: List[str] = []
    # гиперпараметры
    contamination: float

class IsolationForestService:
    def __init__(self) -> None:
        self.plot_service = PlotService()
        self.data_service = DatasetService()
        super().__init__()

    def isolation_forest(self, df, column, settings: IsloationForestSettings):
        x = df[[column]]
        # модель
        # Обучим модель IF на данных
        if_model = IsolationForest(contamination=settings.contamination)  # Параметры можно настраивать
        if_model.fit(x)
        y_pred = if_model.predict(x)
        anomalies_indices = np.where(y_pred == -1)[0]
        # модель
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

    def learn(self, settings: IsloationForestSettings):
        data = pd.read_csv(settings.datasetPath)
        self.data_service.minimal_processing(data, settings.timeColumn)
        for column in settings.columns:
            df = self.data_service.prepareData(data, column, settings.timeColumn, settings.spaceWeatherColumns)
            self.isolation_forest(df.head(5000), column, settings)
