FROM node:18-alpine as build-step

# Set up the working directory
RUN mkdir -p /app
WORKDIR /app

# Copy package.json and install dependencies
COPY package.json /app
RUN npm install

# Copy the rest of the project files and build the Angular application
COPY . /app
RUN npm run build --prod

# Use NGINX to serve the built application
FROM nginx:1.20.1
COPY --from=build-step /app/dist/dashboard /usr/share/nginx/html

# Expose port 80 for NGINX (no need to expose port 4200)
EXPOSE 4200:80