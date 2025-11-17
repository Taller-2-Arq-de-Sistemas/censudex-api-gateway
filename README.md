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
├── .env                        # Variables de entorno del gateway y servicios
└── README.md
```

---

## Configuración

### Variables de Entorno


**Auth Service (.env):**
```env
CLIENTS_SERVICE_URL=http://nginx-gateway:80/clients-api
JWT_SECRET=
JWT_EXPIRATION_MINUTES=
AUTH_SERVICE_PORT=5002
```

**Clients Service (.env):**
```env
DB_CONNECTION_STRING=
AUTH_SERVICE_URL=http://nginx-gateway:80/auth-api
CLIENTS_SERVICE_PORT=5003
RABBITMQ_USER=
RABBITMQ_PASS=
RABBITMQ_HOST=
```

**Inventory Service (.env):**
```env
SUPABASE_URL=
SUPABASE_KEY=
RABBITMQ_URL=
INVENTORY_SERVICE_PORT=5005
```

**Orders Service (.env):**
```env
```

**Products Service (.env):**
```env
DATABASE_URL=
DATABASE_NAME=
COLECTION_NAME=
CLOUDINARY_CLOUD_NAME=
CLOUDINARY_API_KEY=
CLOUDINARY_API_SECRET=
CLOUDINARY_URL=
CLODINARY_FOLDER_NAME=
PRODUCTS_SERVICE_PORT=5004
```

---

## Ejecución del Proyecto

Existen **dos formas** de levantar el entorno completo de Censudex:

1. **Modo Automático (recomendado)** — usando el `Makefile` incluido.
2. **Modo Manual** — paso a paso.

---
### Opción 1: Modo Automático (con Makefile)

Para simplificar proceso de levantamiento el proyecto incluye un `Makefile` que automatiza, para el correcto funcionamiento de `Makefile` en Windows, utiliza la **CLI de Git Bash**:

* La clonación de todos los repositorios
* La copia de los archivos `.env.example`
* Y la construcción/ejecución de los contenedores


#### 0. Clonar repositorio y copiar `.env.example`
Copia y pega los comandos a continuación
```bash
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-api-gateway
cd censudex-api-gateway
cp .env.example .env
```

#### 1. Configurar variables de entorno y ejecutar Docker Engine
Dentro del archivo `.env` generado, copia tus variables de entorno en las variables sin asignar.
Algunas variables, como SERVICE_PORT ya existen por predeterminado, llena aquellas sin un valor asignado.

#### 2. Ejecutar el comando de instalación

Desde la terminal de **Git Bash**:

```bash
make setup
```

> Esto:
> * Clonará automáticamente los repositorios de todos los servicios.
> * Creará los archivos `.env` a partir de los `.env.example`.
> * Construirá y levantará todos los contenedores de Docker.

---

#### 3. Verificar los contenedores

```bash
docker ps
```

Si todo salió bien, deberías ver los mismos servicios levantados que en el modo manual.

---

#### 4. Acceder al sistema

```
http://localhost:5001
```

---

### Opción 2: Modo Manual
#### 1. Clonar todos los repositorios en el **mismo directorio raíz**

```bash
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-api-gateway.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-auth.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-clients.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-inventory.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-orders.git
git clone https://github.com/Taller-2-Arq-de-Sistemas/censudex-products.git
```

> **Importante:** Todos los repositorios deben ubicarse dentro del mismo directorio para que las rutas relativas del `docker-compose.yml` del API Gateway funcionen correctamente.

---

#### 2. Copiar los archivos de configuración de entorno

Cada servicio incluye un archivo de ejemplo llamado `.env.example` o `appsettings.Development.json`.
Debes copiarlo y renombrarlo según corresponda:
> **Antes de ejecutar el siguiente comando, confirma que cada servicio tiene su archivo `.env` completo.** 

```bash
cp .env.example .env
```

o

```bash
cp appsettings.Development.json appsettings.json
```
---

#### 3. Ejecutar el sistema con Docker Compose

Abre la aplicación **Docker Desktop** y desde el directorio del **API Gateway** (`censudex-api-gateway`) ejecuta lo siguiente:

```bash
docker compose up -d --build
```

Esto construirá las imágenes de los servicios y levantará los contenedores necesarios:

* `nginx-gateway` (API Gateway)
* `auth-service`
* `clients-service`
* `inventory-service`
* `products-service`
* `orders-service`

---

#### 4. Verificar el estado de los contenedores

```bash
docker ps
```

Si todo está correctamente configurado, deberías ver los contenedores ejecutándose en segundo plano.

---

#### 5. Acceder al sistema

El **API Gateway** estará disponible en:

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
