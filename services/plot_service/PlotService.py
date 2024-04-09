import os
import matplotlib
matplotlib.use('agg')
from matplotlib import pyplot as plt
plt.ioff()

class PlotService:
    def create_folder_if_not_exist(self, path):
        os.makedirs(path, exist_ok=True)

    def print_scutter_only(self, df, anomaly_indices, column, save_path):
        all_indices = set(df.index)
        non_anomaly_indices = list(all_indices.difference(anomaly_indices))
        plt.figure(figsize=(10, 6))
        plt.scatter(anomaly_indices, df.iloc[anomaly_indices][column], color='red', marker='x', zorder=5, s=20,
                    label='Аномальные точки')
        plt.scatter(non_anomaly_indices, df.iloc[non_anomaly_indices][column], color='green', zorder=1, s=0.5,
                    label='Не аномальные точки')
        plt.xlabel('Индекс')
        plt.ylabel('Значение')
        plt.title(f'Датасет с выделенными аномальными точками {column}')
        plt.legend()
        directory_path = f'{save_path}\\{column}'
        path = f'{directory_path}\\{column}_scutter.jpg'
        self.create_folder_if_not_exist(directory_path)
        plt.savefig(path)

    def print_plot(self, df, anomaly_indices, column, save_path, space_weather = None):
        plt.figure(figsize=(10, 6))
        plt.plot(df.index, df[column], color='blue', label='Обычные точки')
        plt.scatter(anomaly_indices, df.iloc[anomaly_indices][column], color='red', zorder=3,
                    label='Аномальные точки')

        if space_weather != None:
            plt.plot(df.index, df[space_weather], color='orange', label=space_weather)
            plt.scatter(anomaly_indices,
                        df.iloc[anomaly_indices][space_weather],
                        color='red',
                        marker='x',
                        zorder=2,
                        s=10,
                        label=f'Аномальные точки {space_weather}')

        plt.xlabel('Индекс')
        plt.ylabel('Значение')
        if space_weather == None:
            plt.title(f'Датасет с выделенными аномальными точками {column}')
        else:
            plt.title(f'Датасет с выделенными аномальными точками {column} и {space_weather}')
        plt.legend()
        directory_path = f'{save_path}\\{column}'

        if space_weather != None:
            path = f'{directory_path}\\{column}_{space_weather}_plot.jpg'
        else:
            path = f'{directory_path}\\{column}_plot.jpg'

        self.create_folder_if_not_exist(directory_path)
        plt.savefig(path)