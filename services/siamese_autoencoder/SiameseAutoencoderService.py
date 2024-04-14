import pandas as pd
from pydantic import BaseModel
from typing import List
import torch
import torch.nn as nn
import torch.optim as optim
import numpy as np
from torch.utils.data import DataLoader, TensorDataset
from services.plot_service.PlotService import PlotService
from services.dataset_service.DatasetService import DatasetService

class SiameseAutoEncoderSettings(BaseModel):
    datasetPath: str
    savePath: str
    timeColumn: str
    spaceWeatherColumns: List[str] = []
    columns: List[str] = []
    # гиперпараметры

class SiameseAutoencoder(nn.Module):
    def __init__(self, input_dim, latent_dim):
        super(SiameseAutoencoder, self).__init__()
        self.encoder = nn.Sequential(
            nn.Linear(input_dim, 256),
            nn.LeakyReLU(0.2),  # Используем LeakyReLU
            nn.Linear(256, latent_dim),
        )
        self.decoder = nn.Sequential(
            nn.Linear(latent_dim, 256),
            nn.LeakyReLU(0.2),  # Используем LeakyReLU
            nn.Linear(256, input_dim),
        )

    def forward_one(self, x):
        x = self.encoder(x)
        x = self.decoder(x)
        return x

    def forward(self, x1, x2):
        latent1 = self.encoder(x1)
        latent2 = self.encoder(x2)
        return latent1, latent2

class SiameseAutoencoderService:
    def __init__(self) -> None:
        self.plot_service = PlotService()
        self.data_service = DatasetService()
        super().__init__()

    def siamese_autoencoder(self, df, column, settings: SiameseAutoEncoderSettings):
        x = df[[column]]

        #модель
        # Параметры
        input_dim = x.shape[1]  # Ваш размер входных данных
        latent_dim = 2
        batch_size = 64
        learning_rate = 1e-3
        num_epochs = 50

        # Инициализация модели, критерия и оптимизатора
        model = SiameseAutoencoder(input_dim, latent_dim)
        criterion = nn.MSELoss()
        optimizer = optim.Adam(model.parameters(), lr=learning_rate)

        # Преобразование данных в тензор
        data_tensor = torch.tensor(x.values, dtype=torch.float32)

        # Нормализация данных
        mean = torch.mean(data_tensor, dim=0)
        std = torch.std(data_tensor, dim=0)
        data_tensor = (data_tensor - mean) / std

        # Обучение модели
        for epoch in range(num_epochs):
            for i in range(0, len(data_tensor), batch_size):
                batch = data_tensor[i:i + batch_size]
                shuffled_batch = batch[torch.randperm(batch.size(0))]  # Перемешиваем для создания пар
                optimizer.zero_grad()

                output1, output2 = model(batch, shuffled_batch)

                loss = criterion(output1, batch) + criterion(output2, shuffled_batch)
                loss.backward()
                optimizer.step()

            print(f"Epoch [{epoch + 1}/{num_epochs}], Loss: {loss.item():.4f}")

        # Вычисление порога для обнаружения аномалий
        with torch.no_grad():
            latents = model.encoder(data_tensor)
            distances = torch.sum((latents - latents.mean(dim=0)) ** 2, dim=1)
            threshold = torch.mean(distances) + 3 * torch.std(distances)
            anomaly_indices = (distances > threshold).numpy().astype(int)

        anomalies_indices = np.where(anomaly_indices == 1, 1, 0)

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

    def learn(self, settings: SiameseAutoencoder):
        data = pd.read_csv(settings.datasetPath)
        self.data_service.minimal_processing(data, settings.timeColumn)
        for column in settings.columns:
            df = self.data_service.prepareData(data, column, settings.timeColumn, settings.spaceWeatherColumns)
            self.siamese_autoencoder(df.head(5000), column, settings)