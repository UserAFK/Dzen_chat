# Build stage
FROM node:20-alpine AS build
WORKDIR /app
COPY . .
RUN npm install --legacy-peer-deps
RUN npm run build --prod

# Serve stage
FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY default.conf.template /etc/nginx/conf.d/default.conf.template
COPY --from=build /app/dist/dzenchat/browser /usr/share/nginx/html
EXPOSE 8080
CMD ["/bin/sh", "-c", "envsubst '${API_BASE_URL}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf && exec nginx -g 'daemon off;'"]

