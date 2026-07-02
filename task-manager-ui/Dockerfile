# Etapa 1: Compilar Angular con Node
FROM node:22-alpine AS build
WORKDIR /app

# Instalamos dependencias primero (aprovecha la caché de Docker)
COPY package*.json ./
RUN npm install

# Copiamos el código fuente y compilamos para producción
COPY . .
RUN npm run build --configuration=production

# Etapa 2: Servir con NGINX
FROM nginx:alpine
# Borramos la página por defecto de NGINX
RUN rm -rf /usr/share/nginx/html/*

# Copiamos los archivos compilados de Angular al servidor web
# IMPORTANTE: Revisa si tu Angular generó la carpeta 'browser'. Si tu versión de Angular es más vieja, quita '/browser' de la ruta.
COPY --from=build /app/dist/task-manager-ui/browser /usr/share/nginx/html

# Exponemos el puerto del servidor web
EXPOSE 80

# Evitamos que NGINX se ejecute en segundo plano para mantener vivo el contenedor
CMD ["nginx", "-g", "daemon off;"]