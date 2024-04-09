from fastapi import Depends, FastAPI
from fastapi.openapi.utils import get_openapi
from typing import Annotated
from services.dataset_service.DatasetService import DatasetService, DatasetSettings
from services.knn_service.KnnService import KnnSettings, KnnService

# Создаем объект FastAPI
app = FastAPI()
dataset_service = DatasetService()
knn_service = KnnService()

#datasets
@app.get("/datasets/columns")
async def get_columns(path: str):
    return {"columns": dataset_service.get_columns(path)}

@app.post("/datasets/create-on-basis")
async def create_on_basis(datasetSettings: DatasetSettings):
    return {"result": dataset_service.create_on_basis(datasetSettings)}

@app.post("/models/knn")
async def knn(knnSettings: KnnSettings):
    return {"result": knn_service.learn(settings=knnSettings)}


def custom_openapi():
    if app.openapi_schema:
        return app.openapi_schema
    openapi_schema = get_openapi(
        title="FastAPI with Swagger",
        version="1.0.0",
        description="This is a very fancy project, with auto docs for the API and everything",
        routes=app.routes,
    )
    app.openapi_schema = openapi_schema
    return app.openapi_schema

app.openapi = custom_openapi

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)