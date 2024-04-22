import pandas as pd
from pydantic import BaseModel
from typing import List
import numpy as np
from sklearn.neighbors import NearestNeighbors
from sklearn.neighbors import LocalOutlierFactor
from sklearn.ensemble import IsolationForest
from sklearn.mixture import GaussianMixture
from sklearn.neighbors import KernelDensity
from sklearn.svm import OneClassSVM
from sklearn.cluster import DBSCAN
from sklearn.cluster import OPTICS
from scipy.stats import multivariate_normal
from sklearn.cluster import KMeans
from scipy.cluster.hierarchy import linkage, fcluster
import tensorflow as tf
from sklearn.decomposition import PCA
from scipy.spatial import ConvexHull
from sklearn.covariance import EllipticEnvelope
from sklearn.preprocessing import StandardScaler
from services.plot_service.PlotService import PlotService
from services.dataset_service.DatasetService import DatasetService

def knn(df, column):
    X = df[[column]]
    n_neighbors = 5
    algorithm = 'auto'
    percentil = 95
    knn_model = NearestNeighbors(n_neighbors=n_neighbors, algorithm=algorithm)
    knn_model.fit(X)
    distances, _ = knn_model.kneighbors(X)
    k_distance = distances[:, -1]
    threshold = np.percentile(k_distance, percentil)
    anomalies_indices = np.where(k_distance > threshold)[0]
    return anomalies_indices


def lof(df, column):
    X = df[[column]]
    n_neighbors = 5
    algorithm = 'auto'
    metric = 'euclidean'
    contamination = 0.01
    lof_model = LocalOutlierFactor(n_neighbors=n_neighbors, algorithm=algorithm, metric=metric,
                                   contamination=contamination)
    lof_scores = lof_model.fit_predict(X)
    lof_scores = np.abs(lof_scores)
    threshold = np.percentile(lof_scores, 100 * (1 - contamination))
    anomalies_indices = np.where(lof_scores > threshold)[0]
    return anomalies_indices

def isolation_forest(df, column):
    X = df[[column]]
    n_estimators = 100
    contamination = 0.05
    if_model = IsolationForest(n_estimators=n_estimators, contamination=contamination)
    if_model.fit(X)
    anomaly_scores = if_model.decision_function(X)
    anomaly_scores = -anomaly_scores
    threshold = np.percentile(anomaly_scores, 100 * (1 - contamination))
    anomalies_indices = np.where(anomaly_scores > threshold)[0]
    return anomalies_indices

def gmm_anomaly_detection(df, column):
    X = df[[column]]
    n_components = 3
    gmm = GaussianMixture(n_components=n_components)
    gmm.fit(X)
    log_prob = gmm.score_samples(X)
    anomaly_scores = -log_prob
    threshold = np.percentile(anomaly_scores, 95)
    anomalies_indices = np.where(anomaly_scores > threshold)[0]
    return anomalies_indices


def kde_anomaly_detection(df, column):
    X = df[[column]].values
    kernel = 'gaussian'
    bandwidth = 0.75
    kde = KernelDensity(kernel=kernel, bandwidth=bandwidth)
    kde.fit(X)
    log_density = kde.score_samples(X)
    anomaly_scores = -log_density
    threshold = np.percentile(anomaly_scores, 95)
    anomalies_indices = np.where(anomaly_scores > threshold)[0]
    return anomalies_indices

def ocsvm_anomaly_detection(df, column):
    X = df[[column]]
    kernel = 'rbf'
    nu = 0.05
    ocsvm = OneClassSVM(kernel=kernel, nu=nu)
    ocsvm.fit(X)
    anomaly_scores = ocsvm.decision_function(X)
    anomaly_scores = -anomaly_scores
    threshold = np.percentile(anomaly_scores, 95)
    anomalies_indices = np.where(anomaly_scores > threshold)[0]
    return anomalies_indices

def dbscan_anomaly_detection(df, column):
    X = df[[column]].values
    eps = 0.5
    min_samples = 5
    dbscan = DBSCAN(eps=eps, min_samples=min_samples)
    cluster_labels = dbscan.fit_predict(X)
    anomalies_indices = np.where(cluster_labels == -1)[0]
    return anomalies_indices

def optics_anomaly_detection(df, column):
    X = df[[column]].values
    min_samples = 5
    xi = 0.05
    optics = OPTICS(min_samples=min_samples, xi=xi)
    cluster_labels = optics.fit_predict(X)
    anomalies_indices = np.where(cluster_labels == -1)[0]
    return anomalies_indices

def gad_anomaly_detection(df, column):
    X = df[[column]].values
    mean = np.mean(X, axis=0)
    cov = np.cov(X, rowvar=False)
    mvn = multivariate_normal(mean=mean, cov=cov)
    anomaly_scores = mvn.pdf(X)
    threshold = np.percentile(anomaly_scores, 95)
    anomalies_indices = np.where(anomaly_scores < threshold)[0]
    return anomalies_indices


def kmad_anomaly_detection(df, column):
    X = df[[column]].values
    n_clusters = 3
    kmeans = KMeans(n_clusters=n_clusters)
    cluster_labels = kmeans.fit_predict(X)
    cluster_centers = kmeans.cluster_centers_
    distances = [np.linalg.norm(x - cluster_centers[cluster_labels[i]]) for i, x in enumerate(X)]
    threshold = np.percentile(distances, 95)
    anomalies_indices = np.where(distances > threshold)[0]
    return anomalies_indices


def had_anomaly_detection(df, column):
    X = df[[column]].values
    Z = linkage(X, method='ward')
    max_clusters = 3
    cluster_labels = fcluster(Z, max_clusters, criterion='maxclust')
    cluster_means = [np.mean(X[cluster_labels == i]) for i in range(1, max_clusters + 1)]
    distances = [np.abs(x - cluster_means[cluster_labels[i] - 1]) for i, x in enumerate(X)]
    threshold = np.percentile(distances, 95)
    anomalies_indices = np.where(distances > threshold)[0]
    return anomalies_indices


def sad_anomaly_detection(df, column):
    X = df[[column, 'time_epoch']].values
    n_clusters = 3
    n_subspaces = 2
    anomalies_indices = []
    features_per_subspace = int(X.shape[1] / n_subspaces)

    for i in range(n_subspaces):
        start_idx = i * features_per_subspace
        end_idx = (i + 1) * features_per_subspace
        X_subspace = X[:, start_idx:end_idx]
        kmeans = KMeans(n_clusters=n_clusters)
        cluster_labels = kmeans.fit_predict(X_subspace)
        cluster_centers = kmeans.cluster_centers_
        distances = [np.linalg.norm(x_sub - cluster_centers[cluster_labels[i]]) for i, x_sub in enumerate(X_subspace)]
        threshold = np.percentile(distances, 95)
        anomalies_indices_subspace = np.where(distances > threshold)[0]
        anomalies_indices.extend(anomalies_indices_subspace)
    anomalies_indices = list(set(anomalies_indices))

    return anomalies_indices


def build_autoencoder(input_dim, latent_dim):
    # Входной слой
    input_layer = tf.keras.Input(shape=(input_dim,))

    # Энкодер
    encoder = tf.keras.layers.Dense(units=64, activation='relu')(input_layer)
    encoder = tf.keras.layers.Dense(units=32, activation='relu')(encoder)
    encoder = tf.keras.layers.Dense(units=latent_dim, activation='relu')(encoder)

    # Декодер
    decoder = tf.keras.layers.Dense(units=32, activation='relu')(encoder)
    decoder = tf.keras.layers.Dense(units=64, activation='relu')(decoder)
    decoder = tf.keras.layers.Dense(units=input_dim, activation='sigmoid')(decoder)

    # Собираем модель
    autoencoder = tf.keras.Model(inputs=input_layer, outputs=decoder)

    return autoencoder


def aad(df, column):
    X = df[[column]].values
    input_dim = X.shape[1]
    autoencoder = build_autoencoder(input_dim, 10)
    autoencoder.compile(optimizer='adam', loss='mean_squared_error')
    autoencoder.fit(X, X, epochs=50, batch_size=32, shuffle=True, validation_split=0.2)
    reconstructed_X = autoencoder.predict(X)
    reconstruction_error = np.mean(np.square(X - reconstructed_X), axis=1)
    threshold = np.percentile(reconstruction_error, 95)
    anomalies_indices = np.where(reconstruction_error > threshold)[0]

    return anomalies_indices


def pcad(df, column):
    X = df[[column]].values
    pca = PCA(n_components=1)
    pca.fit(X)
    transformed_X = pca.transform(X)
    reconstructed_X = pca.inverse_transform(transformed_X)
    reconstruction_error = np.mean(np.square(X - reconstructed_X), axis=1)
    threshold = np.percentile(reconstruction_error, 95)
    anomalies_indices = np.where(reconstruction_error > threshold)[0]

    return anomalies_indices


def svd_anomaly_detection(df, column):
    X = df[[column]].values
    U, s, Vt = np.linalg.svd(X, full_matrices=False)
    principal_component = U[:, 0]
    reconstructed_X = np.outer(principal_component, Vt[0])
    reconstruction_error = np.mean(np.square(X - reconstructed_X), axis=1)
    threshold = np.percentile(reconstruction_error, 95)
    anomalies_indices = np.where(reconstruction_error > threshold)[0]
    return anomalies_indices

def chad(df, column):
    X = df[['index', column]].values
    hull = ConvexHull(X)
    hull_vertices = hull.vertices
    distances = np.array([min(np.linalg.norm(x - X[hull_vertices], axis=1)) for x in X])
    threshold = np.percentile(distances, 95)
    anomalies_indices = np.where(distances > threshold)[0]
    return anomalies_indices


def mvad(df, column):
    X = df[[column]].values
    clf = EllipticEnvelope(contamination=0.05)
    clf.fit(X)
    predictions = clf.predict(X)
    anomalies_indices = np.where(predictions == -1)[0]
    return anomalies_indices


def hsad(df, column):
    X = df[[column]].values
    mean = np.mean(X, axis=0)
    cov_matrix = np.cov(X, rowvar=False)
    inv_cov_matrix = np.linalg.inv(cov_matrix)
    normal_vector = np.dot(inv_cov_matrix, mean)
    distances = np.dot(X - mean, normal_vector)
    threshold = np.percentile(distances, 95)
    anomalies_indices = np.where(distances > threshold)[0]
    return anomalies_indices

def feature_bagging_anomaly_detection(df, column):
    X = df[[column]].values
    clf = IsolationForest(contamination=0.05)
    clf.fit(X)
    predictions = clf.predict(X)
    anomalies_indices = np.where(predictions == -1)[0]
    return anomalies_indices


def ife_anomaly_detection(df, column):
    X = df[[column]].values
    n_estimators = 10
    predictions_list = []
    for _ in range(n_estimators):
        clf = IsolationForest(contamination=0.05)
        clf.fit(X)
        predictions = clf.predict(X)
        predictions_list.append(predictions)

    predictions_array = np.array(predictions_list)
    final_predictions = np.sum(predictions_array, axis=0)
    anomalies_indices = np.where(final_predictions < 0)[0]

    return anomalies_indices


def admc_anomaly_detection(df, column):
    X = df[[column]].values
    lof = LocalOutlierFactor(n_neighbors=20, contamination=0.05)
    iforest = IsolationForest(contamination=0.05)
    svm = OneClassSVM(nu=0.05)
    predictions_lof = lof.fit_predict(X)
    predictions_iforest = iforest.fit_predict(X)
    predictions_svm = svm.fit_predict(X)
    combined_predictions = predictions_lof + predictions_iforest + predictions_svm
    anomalies_indices = np.where(combined_predictions < -1)[0]
    return anomalies_indices

def autoencoder_anomaly_detection(df, column):
    X = df[[column]].values
    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X)
    input_dim = X_scaled.shape[1]
    encoding_dim = 32
    input_layer = tf.keras.layers.Input(shape=(input_dim,))
    encoder = tf.keras.layers.Dense(encoding_dim, activation='relu')(input_layer)
    decoder = tf.keras.layers.Dense(input_dim, activation='sigmoid')(encoder)
    autoencoder = tf.keras.models.Model(inputs=input_layer, outputs=decoder)
    autoencoder.compile(optimizer='adam', loss='mean_squared_error')
    autoencoder.fit(X_scaled, X_scaled, epochs=50, batch_size=32, shuffle=True, validation_split=0.2)
    reconstructions = autoencoder.predict(X_scaled)
    mse = np.mean(np.power(X_scaled - reconstructions, 2), axis=1)
    percentil = 95
    threshold = np.percentile(mse, percentil)
    anomalies_indices = np.where(mse > threshold)[0]
    return anomalies_indices

def ffnn_anomaly_detection(df, column):
    X = df[[column]].values
    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X)
    input_dim = X_scaled.shape[1]
    hidden_dim = 32
    output_dim = 1
    model = tf.keras.Sequential([
        tf.keras.layers.Input(shape=(input_dim,)),
        tf.keras.layers.Dense(hidden_dim, activation='relu'),
        tf.keras.layers.Dense(output_dim)
    ])
    model.compile(optimizer='adam', loss='mse')
    model.fit(X_scaled, X_scaled, epochs=50, batch_size=32, shuffle=True, validation_split=0.2)
    predictions = model.predict(X_scaled)
    mse = np.mean(np.power(X_scaled - predictions, 2), axis=1)
    percentil = 95
    threshold = np.percentile(mse, percentil)
    anomalies_indices = np.where(mse > threshold)[0]

    return anomalies_indices

def build_generator(input_dim):
    model = tf.keras.Sequential([
        tf.keras.layers.Dense(32, activation='relu', input_shape=(input_dim,)),
        tf.keras.layers.Dense(input_dim, activation='sigmoid')
    ])
    return model

def build_discriminator(input_dim):
    model = tf.keras.Sequential([
        tf.keras.layers.Dense(32, activation='relu', input_shape=(input_dim,)),
        tf.keras.layers.Dense(1, activation='sigmoid')
    ])
    model.compile(optimizer='adam', loss='binary_crossentropy', metrics=['accuracy'])
    return model

def build_gan(generator, discriminator):
    discriminator.trainable = False
    model = tf.keras.Sequential([
        generator,
        discriminator
    ])
    model.compile(optimizer='adam', loss='binary_crossentropy')
    return model

def gan_anomaly_detection(df, column, epochs=100, batch_size=32):
    X = df[[column]].values
    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X)
    input_dim = X_scaled.shape[1]
    generator = build_generator(input_dim)
    discriminator = build_discriminator(input_dim)
    gan = build_gan(generator, discriminator)
    for epoch in range(epochs):
        noise = np.random.normal(0, 1, (batch_size, input_dim))
        generated_data = generator.predict(noise)
        real_data = X_scaled[np.random.randint(0, X_scaled.shape[0], batch_size)]
        discriminator.trainable = True
        d_loss_real = discriminator.train_on_batch(real_data, np.ones((batch_size, 1)))
        d_loss_fake = discriminator.train_on_batch(generated_data, np.zeros((batch_size, 1)))
        d_loss = 0.5 * np.add(d_loss_real, d_loss_fake)
        discriminator.trainable = False
        g_loss = gan.train_on_batch(noise, np.ones((batch_size, 1)))
    generated_data = generator.predict(X_scaled)
    mse = np.mean(np.power(X_scaled - generated_data, 2), axis=1)
    percentil = 95
    threshold = np.percentile(mse, percentil)
    anomalies_indices = np.where(mse > threshold)[0]
    return anomalies_indices

def build_rnn(input_shape):
    model = tf.keras.Sequential([
        tf.keras.layers.SimpleRNN(32, activation='relu', input_shape=input_shape, return_sequences=True),
        tf.keras.layers.SimpleRNN(16, activation='relu', return_sequences=True),
        tf.keras.layers.Dense(1)
    ])
    model.compile(optimizer='adam', loss='mse')
    return model

def rnn_anomaly_detection(df, column, sequence_length=5, epochs=50, batch_size=32):
    X = df[[column]].values
    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X)
    sequences = []
    for i in range(len(X_scaled) - sequence_length):
        sequences.append(X_scaled[i:i + sequence_length])
    sequences = np.array(sequences)
    train_size = int(0.8 * len(sequences))
    train_data = sequences[:train_size]
    test_data = sequences[train_size:]
    X_train = train_data[:, :-1]
    y_train = train_data[:, -1]
    X_test = test_data[:, :-1]
    y_test = test_data[:, -1]
    input_shape = (X_train.shape[1], 1)
    model = build_rnn(input_shape)
    model.fit(X_train, y_train, epochs=epochs, batch_size=batch_size, shuffle=True)
    predictions = model.predict(X_test)
    mse = np.mean(np.power(y_test - predictions[:, 0], 2), axis=1)
    percentil = 95
    threshold = np.percentile(mse, percentil)
    anomalies_indices = np.where(mse > threshold)[0] + train_size
    return anomalies_indices


def build_cnn(input_shape):
    model = tf.keras.Sequential([
        tf.keras.layers.Conv1D(filters=32, kernel_size=3, activation='relu', input_shape=input_shape),
        tf.keras.layers.MaxPooling1D(pool_size=2),
        tf.keras.layers.Flatten(),
        tf.keras.layers.Dense(50, activation='relu'),
        tf.keras.layers.Dense(1)
    ])
    model.compile(optimizer='adam', loss='mse')
    return model


def cnn_anomaly_detection(df, column, sequence_length=5, epochs=50, batch_size=32):
    X = df[[column]].values
    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X)
    sequences = []
    for i in range(len(X_scaled) - sequence_length):
        sequences.append(X_scaled[i:i + sequence_length])
    sequences = np.array(sequences)
    train_size = int(0.8 * len(sequences))
    train_data = sequences[:train_size]
    test_data = sequences[train_size:]
    X_train = train_data[:, :-1]
    y_train = train_data[:, -1]
    X_test = test_data[:, :-1]
    y_test = test_data[:, -1]
    input_shape = (X_train.shape[1], 1)
    X_train = np.reshape(X_train, (X_train.shape[0], X_train.shape[1], 1))
    X_test = np.reshape(X_test, (X_test.shape[0], X_test.shape[1], 1))
    model = build_cnn(input_shape)
    model.fit(X_train, y_train, epochs=epochs, batch_size=batch_size, shuffle=True)
    predictions = model.predict(X_test)
    mse = np.mean(np.power(y_test - predictions[:, 0], 2), axis=1)
    percentil = 95
    threshold = np.percentile(mse, percentil)
    anomalies_indices = np.where(mse > threshold)[0] + train_size
    return anomalies_indices

def model_factory(type):
    if type == 1:
        return knn
    if type == 2:
        return lof
    if type == 3:
        return isolation_forest
    if type == 4:
        return gmm_anomaly_detection
    if type == 5:
        return kde_anomaly_detection
    if type == 6:
        return ocsvm_anomaly_detection
    if type == 7:
        return dbscan_anomaly_detection
    if type == 8:
        return optics_anomaly_detection
    if type == 9:
        return gad_anomaly_detection
    if type == 10:
        return kmad_anomaly_detection
    if type == 11:
        return had_anomaly_detection
    if type == 12:
        return sad_anomaly_detection #пока что не работает
    if type == 13:
        return aad
    if type == 14:
        return pcad
    if type == 15:
        return svd_anomaly_detection
    if type == 16:
        return chad
    if type == 17:
        return mvad
    if type == 18:
        return hsad #пока что не работает
    if type == 19:
        return feature_bagging_anomaly_detection
    if type == 20:
        return ife_anomaly_detection
    if type == 21:
        return admc_anomaly_detection
    if type == 22:
        return autoencoder_anomaly_detection
    if type == 23:
        return ffnn_anomaly_detection
    if type == 24:
        return gan_anomaly_detection
    if type == 25:
        return rnn_anomaly_detection
    if type == 26:
        return cnn_anomaly_detection

class ModelSettings(BaseModel):
    modelType: int
    datasetPath: str
    savePath: str
    timeColumn: str
    spaceWeatherColumns: List[str] = []
    columns: List[str] = []

class ModelsService:
    def __init__(self) -> None:
        self.plot_service = PlotService()
        self.data_service = DatasetService()
        super().__init__()

    def run_model(self, df, column, settings: ModelSettings):
        model = model_factory(settings.modelType)
        anomalies_indices = model(df, column)
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


    def learn(self, settings: ModelSettings):
        data = pd.read_csv(settings.datasetPath)
        for column in settings.columns:
            df = self.data_service.prepareData(data, column, settings.timeColumn, settings.spaceWeatherColumns)
            df.dropna(axis=1, how='any', inplace=True)
            print(df)
            df[column] = df[column].astype(float)
            self.run_model(df, column, settings)