# Estágio 1 — build
FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# Estágio 2 — runtime
FROM node:22-alpine AS runtime
WORKDIR /app

COPY --from=build /app/dist/products-app/server ./dist/products-app/server
COPY --from=build /app/dist/products-app/browser ./dist/products-app/browser

ENV PORT=4000
EXPOSE 4000

CMD ["node", "dist/products-app/server/server.mjs"]
