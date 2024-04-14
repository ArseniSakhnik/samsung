import pandas as pd
from pydantic import BaseModel
from typing import List
import numpy as np
import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, TensorDataset
from services.plot_service.PlotService import PlotService
from services.dataset_service.DatasetService import DatasetService

class Autoencoder(nn.Module):
    def __init__(self, input_dim, latent_dim):
        super(Autoencoder, self).__init__()
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

    def forward(self, x):
        x = self.encoder(x)
        x = self.decoder(x)
        return x

class AutoEncoderSettings(BaseModel):
    datasetPath: str
    savePath: str
    timeColumn: str
    spaceWeatherColumns: List[str] = []
    columns: List[str] = []
    # гиперпараметры

class AutoEncoderService:
    def __init__(self) -> None:
        self.plot_service = PlotService()
        self.data_service = DatasetService()
        super().__init__()

    def autoencoder(self, df, column, settings: AutoEncoderSettings):
        x = df[[column]]

        #модель
        input_dim = x.shape[1]
        data_tensor = torch.tensor(x.values, dtype=torch.float32)
        mean = torch.mean(data_tensor, dim=0)
        std = torch.std(data_tensor, dim=0)
        data_tensor = (data_tensor - mean) / std
        latent_dim = 2
        batch_size = 64
        learning_rate = 1e-3
        num_epochs = 50
        model = Autoencoder(input_dim, latent_dim)
        criterion = nn.MSELoss()
        optimizer = optim.Adam(model.parameters(), lr=learning_rate)

        for epoch in range(num_epochs):
            for i in range(0, len(data_tensor), batch_size):
                batch = data_tensor[i:i + batch_size]
                optimizer.zero_grad()
                outputs = model(batch)
                loss = criterion(outputs, batch)
                loss.backward()
                optimizer.step()
            print(f"Epoch [{epoch + 1}/{num_epochs}], Loss: {loss.item():.4f}")


        with torch.no_grad():
            reconstructed = model(data_tensor)
            reconstruction_loss = torch.mean((data_tensor - reconstructed) ** 2, dim=1)
            threshold = torch.mean(reconstruction_loss) + 3 * torch.std(reconstruction_loss)
            anomaly_indices = (reconstruction_loss > threshold).numpy().astype(int)

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

    def learn(self, settings: AutoEncoderSettings):
        data = pd.read_csv(settings.datasetPath)
        self.data_service.minimal_processing(data, settings.timeColumn)
        for column in settings.columns:
            df = self.data_service.prepareData(data, column, settings.timeColumn, settings.spaceWeatherColumns)
            self.autoencoder(df, column, settings)