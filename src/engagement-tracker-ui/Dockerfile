# ─────────────────────────────────────────
# Stage 1: Build the Angular application
# ─────────────────────────────────────────
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files first for layer caching
COPY package.json package-lock.json ./
RUN npm ci

# Copy source and build for production
COPY . .
RUN npm run build -- --configuration production

# ─────────────────────────────────────────
# Stage 2: Serve with nginx (minimal image)
# ─────────────────────────────────────────
FROM nginx:alpine AS runtime

# Custom nginx config for Angular SPA routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built Angular app from build stage
COPY --from=build /app/dist/engagement-tracker-ui/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
