# Censudex API Gateway

## Descripción

Este proyecto forma parte del ecosistema **Censudex**, una arquitectura basada en microservicios.  
El **API Gateway** actúa como punto de entrada único para todas las solicitudes externas hacia los distintos microservicios (como [`Clients`](https://github.com/Taller-2-Arq-de-Sistemas/censudex-clients), [`Auth`](https://github.com/Taller-2-Arq-de-Sistemas/censudex-auth), [`Products`](https://github.com/Taller-2-Arq-de-Sistemas/censudex-products), [`Inventory`](https://github.com/Taller-2-Arq-de-Sistemas/censudex-inventory), y [`Orders`](https://github.com/Taller-2-Arq-de-Sistemas/censudex-orders)).

Se encarga de enrutar, autenticar y unificar la comunicación entre los servicios, asegurando un acceso seguro y escalable al sistema completo.

---

## Arquitectura Actual (Docker + Nginx)

El **Censudex API Gateway** está construido sobre **Nginx** como reverse proxy y utiliza **Docker Compose** para la orquestación de contenedores.

### Componentes principales:
- **Nginx** → Reverse proxy y enrutamiento
- **Docker Compose** → Orquestación de contenedores
- **Microservicios .NET** → Auth, Clients, Products, Inventory, Orders

---

## Estructura del Proyecto

```
censudex-api-gateway/
├── nginx/
│   ├── nginx.conf              # Configuración principal de Nginx
│   └── conf.d/                 # Configuraciones adicionales
├── logs/                       # Logs de Nginx
├── docker-compose.yml          # Orquestación de contenedores
├── .env                        # Variables de entorno del gateway
└── README.md
```

---

## Configuración

### Variables de Entorno


**Auth Service (.env):**
```env
CLIENTS_SERVICE_URL=http://nginx-gateway:80/clients-api
JWT_SECRET=your-api-secret
JWT_EXPIRATION_MINUTES=your-preferred-time
```

**Clients Service (.env):**
```env
DB_CONNECTION_STRING=your-db-connection-string
AUTH_SERVICE_URL=http://nginx-gateway:80/auth-api
```

**Inventory Service (.env):**
```env
SUPABASE_URL=your-db-url
SUPABASE_KEY=your-db-key
```

**Orders Service (.env):**
```env
```

**Products Service (.env):**
```env
```

---

## Ejecución del Proyecto

### 1. Clonar todos los repositorios

```bash
# Clonar el API Gateway
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-api-gateway.git
cd censudex-api-gateway

# Clonar los microservicios (en el mismo directorio padre)
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-auth.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-clients.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-inventory.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-orders.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-products.git
```


### 2. Ejecutar con Docker Compose

```bash
docker-compose up -d
```

### 3. Verificar los servicios

```bash
docker ps
```

El Gateway estará disponible en:
```
http://localhost:5001
```

---

## Endpoints Disponibles

| Servicio      | Ruta Base        | Puerto Interno | Ejemplo de Endpoint          |
| ------------- | ---------------- | -------------- | ---------------------------- |
| **Auth**      | `/auth-api`      | `5002`         | `POST /auth-api/auth/login`  |
| **Clients**   | `/clients-api`   | `5003`         | `GET /clients-api/clients`   |
| **Products**  | `/products-api`  | `5004`         | `GET /products-api/products` |
| **Inventory** | `/inventory-api` | `5005`         | `GET /inventory-api/items`   |
| **Orders**    | `/orders-api`    | `5006`         | `POST /orders-api/orders`    |

---

## Autenticación

El gateway implementa autenticación JWT mediante un endpoint interno:

- **Endpoint de validación interno**: `/_auth` (solo accesible por Nginx)
- **Flujo de autenticación**:
  1. Cliente envía token en header `Authorization`
  2. Nginx hace subrequest interno a `/_auth`
  3. Auth service valida el token
  4. Si es válido (200), la request continúa
  5. Si es inválido (401/403), se bloquea la request

---

## Desarrollo

### Estructura de Nginx Configuration

El archivo `nginx/nginx.conf` contiene:

- **Upstreams**: Definición de servicios backend
- **Rate Limiting**: Limitación de requests por IP
- **Routing**: Enrutamiento basado en paths
- **Auth Subrequests**: Validación interna de tokens

### Agregar un nuevo microservicio

1. Agregar servicio en `docker-compose.yml`
2. Agregar upstream en `nginx.conf`
3. Configurar rutas en `nginx.conf`
4. Actualizar variables de entorno

---

## Monitoreo

### Logs de Nginx

```bash
docker logs nginx-gateway
```

### Logs de servicios específicos

```bash
docker logs auth-service
docker logs clients-service
```

### Health Check

```bash
curl http://localhost:5001/health
```
