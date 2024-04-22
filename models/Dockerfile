FROM python:3.9 AS build-env
WORKDIR /code
RUN pip install pandas numpy scikit-learn matplotlib seaborn tensorflow

FROM build-env
WORKDIR /code
COPY . .
RUN pip install uvicorn fastapi
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "80"]