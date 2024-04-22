FROM node:16-alpine3.17 as builder
WORKDIR /app
COPY . .
RUN npm install && \
    npm run build 

FROM nginx:1.25.3
COPY --from=builder /app/dist /usr/share/nginx/html