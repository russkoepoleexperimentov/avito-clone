# ResalePlatform

Fullstack-маркетплейс для продажи вещей между частными лицами (клон Авито).
Pet-проект для портфолио.

**Стек:** React + TypeScript + Vite + Tailwind (frontend) · ASP.NET Core Web API .NET 9 ·
PostgreSQL + EF Core · Clean Architecture (CQRS/MediatR) · JWT-аутентификация.

---

## Требования к окружению

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) и npm
- [Docker](https://www.docker.com/) (для PostgreSQL)
- Инструмент EF Core CLI (для миграций):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## Запуск локально

Приложение состоит из трёх частей: база данных, backend API и frontend.
Нужно запустить все три.

### 1. База данных (PostgreSQL в Docker)

```bash
docker compose up -d
```

Поднимает PostgreSQL 17 на порту **5433** (5432 намеренно свободен под локальный
нативный Postgres, если он установлен). Данные: БД `resaleplatform`, пользователь
`resale`, пароль `resale`.

### 2. Backend API

```bash
cd backend
dotnet run --project src/ResalePlatform.API
```

API поднимется на **http://localhost:5080**. При старте автоматически применяются
миграции и наполняются начальные данные (роли + администратор).

- Swagger UI: http://localhost:5080/swagger
- Health-check: http://localhost:5080/health

### 3. Frontend

```bash
cd frontend
npm install   # только при первом запуске
npm run dev
```

Откроется на **http://localhost:5173**. Запросы к `/api` проксируются на backend
(см. `vite.config.ts`), поэтому backend должен быть запущен — иначе будет `502 Bad Gateway`.

---

## Учётная запись администратора (seed)

| Поле | Значение |
|------|----------|
| Email | `admin@resale.local` |
| Пароль | `Admin123$` |

Задаются в `backend/src/ResalePlatform.API/appsettings.json` (секция `Seed`).

---

## Структура проекта

```
ResalePlatform/
├─ backend/
│  └─ src/
│     ├─ ResalePlatform.Domain/          # сущности, enums (без зависимостей)
│     ├─ ResalePlatform.Application/     # CQRS (MediatR), DTO, интерфейсы, валидаторы
│     ├─ ResalePlatform.Infrastructure/  # EF Core, PostgreSQL, Identity, JWT, миграции
│     └─ ResalePlatform.API/             # контроллеры, middleware, DI, Swagger
├─ frontend/
│  └─ src/
│     ├─ app/          # провайдеры (Router, React Query)
│     ├─ components/   # layout, общие компоненты
│     ├─ features/     # фичи по доменам (auth, ...)
│     ├─ lib/          # axios-клиент, query client, утилиты
│     └─ pages/        # страницы-роуты
├─ docs/
│  └─ DATA-MODEL.md    # модель данных и связи
├─ docker-compose.yml  # PostgreSQL
├─ REQUIREMENTS.md     # требования и roadmap
└─ README.md
```

---

## Статус

Проект в разработке. Готово:

- [x] Каркас: Clean Architecture (backend) + React/Tailwind (frontend)
- [x] Модель данных и первая миграция
- [x] Аутентификация: регистрация / вход / JWT + refresh-токены с ротацией
- [ ] Категории (CRUD + seed дерева)
- [ ] Объявления (CRUD + загрузка фото)
- [ ] Каталог: поиск, фильтры, пагинация
- [ ] Избранное
- [ ] Сообщения (чат)

Подробности — в [REQUIREMENTS.md](REQUIREMENTS.md).
