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

class GanSettings(BaseModel):
    datasetPath: str
    savePath: str
    timeColumn: str
    spaceWeatherColumns: List[str] = []
    columns: List[str] = []
    # гиперпараметры

# Определение генератора
class Generator(nn.Module):
    def __init__(self, input_dim, latent_dim):
        super(Generator, self).__init__()
        self.model = nn.Sequential(
            nn.Linear(latent_dim, 256),
            nn.LeakyReLU(0.2),
            nn.Linear(256, input_dim)
        )

    def forward(self, x):
        return self.model(x)

class Discriminator(nn.Module):
    def __init__(self, input_dim, latent_dim):
        super(Discriminator, self).__init__()
        self.model = nn.Sequential(
            nn.Linear(input_dim, 256),
            nn.LeakyReLU(0.2),
            nn.Linear(256, latent_dim),
            nn.LeakyReLU(0.2),
            nn.Linear(latent_dim, 1),
            nn.Sigmoid()
        )

    def forward(self, x):
        return self.model(x)

class GanService:
    def __init__(self) -> None:
        self.plot_service = PlotService()
        self.data_service = DatasetService()
        super().__init__()

    def gan(self, df, column, settings: GanSettings):
        x = df[[column]]

        #модель
        # Параметры
        input_dim = x.shape[1]  # Ваш размер входных данных
        latent_dim = 2
        batch_size = 8
        learning_rate = 1e-3
        num_epochs = 50

        # Инициализация модели, критерия и оптимизатора
        generator = Generator(input_dim, latent_dim)
        discriminator = Discriminator(input_dim, latent_dim)
        criterion = nn.BCELoss()
        optimizer_g = optim.Adam(generator.parameters(), lr=learning_rate)
        optimizer_d = optim.Adam(discriminator.parameters(), lr=learning_rate)

        # Преобразование данных в тензор
        data_tensor = torch.tensor(x.values, dtype=torch.float32)

        # Нормализация данных
        mean = torch.mean(data_tensor, dim=0)
        std = torch.std(data_tensor, dim=0)
        data_tensor = (data_tensor - mean) / std

        # Обучение GAN
        for epoch in range(num_epochs):
            for i in range(0, len(data_tensor), batch_size):
                real_data = data_tensor[i:i + batch_size]

                # Обучение дискриминатора
                optimizer_d.zero_grad()

                # Генерация фейковых данных
                z = torch.randn(batch_size, latent_dim)
                fake_data = generator(z)

                real_labels = torch.ones(batch_size, 1)
                fake_labels = torch.zeros(batch_size, 1)

                outputs_real = discriminator(real_data)
                outputs_fake = discriminator(fake_data.detach())

                loss_d = criterion(outputs_real, real_labels) + criterion(outputs_fake, fake_labels)
                loss_d.backward()
                optimizer_d.step()

                # Обучение генератора
                optimizer_g.zero_grad()
                outputs = discriminator(fake_data)
                loss_g = criterion(outputs, real_labels)
                loss_g.backward()
                optimizer_g.step()

        # Вычисление порога для обнаружения аномалий
        with torch.no_grad():
            z = torch.randn(len(data_tensor), latent_dim)
            fake_data = generator(z)
            distances = torch.sum((data_tensor - fake_data) ** 2, dim=1)
            threshold = torch.mean(distances) + 3 * torch.std(distances)
            anomaly_indices = (distances > threshold).numpy().astype(int)

        anomalies_indices = np.where(anomaly_indices == 1, 1, 0)
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

    def learn(self, settings: GanSettings):
        data = pd.read_csv(settings.datasetPath)
        self.data_service.minimal_processing(data, settings.timeColumn)
        for column in settings.columns:
            df = self.data_service.prepareData(data, column, settings.timeColumn, settings.spaceWeatherColumns)
            self.gan(df, column, settings)