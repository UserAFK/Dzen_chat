# Build stage
FROM node:20-alpine AS build
WORKDIR /app
COPY . .
RUN npm install --legacy-peer-deps
RUN npm run build --prod

# Serve stage
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
# COPY --from=build /app/dist/dzenchat /usr/share/nginx/html
COPY --from=build /app/dist/dzenchat/browser /usr/share/nginx/html
# COPY --from=build /app/dist/frontend/browser /usr/share/nginx/htm
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
