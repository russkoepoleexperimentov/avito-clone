# ResalePlatform — модель данных

БД: **PostgreSQL**. ORM: **EF Core** (Code First, миграции).
Соглашения: имена таблиц во множественном числе (`listings`), snake_case в БД
(через `UseSnakeCaseNamingConvention`), PascalCase в C#. PK — `Guid` (кроме Identity, где по умолчанию `string`).

---

## 1. ER-диаграмма (связи)

```
                ┌──────────────┐
                │  AspNetUsers │ (Identity)
                │  User        │
                └──────┬───────┘
                       │ 1
          ┌────────────┼───────────────┬──────────────┐
          │ N          │ N             │ N            │ N (as buyer/seller)
   ┌──────▼─────┐ ┌────▼──────┐ ┌──────▼──────┐ ┌─────▼─────────┐
   │  Listing   │ │ Favorite  │ │  Message    │ │ Conversation  │
   └──────┬─────┘ └────┬──────┘ └──────┬──────┘ └─────┬─────────┘
          │ 1          │ N              │ N            │ 1
          │ N     ┌────▼──────┐         └─────────────►│
   ┌──────▼─────┐ │ (listing) │        Conversation 1──┘ N Message
   │ListingImage│ └───────────┘
   └────────────┘
          │ N
          │
   ┌──────▼─────┐
   │  Category  │◄──┐ self-reference (ParentId)
   └────────────┘───┘
```

Кратко по связям:
- **User 1—N Listing** — у пользователя много объявлений.
- **Category 1—N Listing** — объявление принадлежит одной категории.
- **Category 1—N Category** — самоссылка (родитель → подкатегории).
- **Listing 1—N ListingImage** — у объявления несколько фото.
- **User N—N Listing** через **Favorite** — избранное.
- **Conversation** привязан к Listing + двум User (buyer, seller); **1—N Message**.

---

## 2. Сущности

### User (наследует IdentityUser<Guid>)
Расширяем стандартного пользователя ASP.NET Identity.

| Поле | Тип | Ограничения |
|------|-----|-------------|
| Id | Guid | PK (из IdentityUser) |
| Email | string | из Identity, уникальный |
| PasswordHash | string | из Identity |
| DisplayName | string(60) | not null |
| Phone | string(20) | nullable |
| City | string(80) | nullable |
| AvatarUrl | string(500) | nullable |
| IsBlocked | bool | default false |
| CreatedAt | DateTimeOffset | default now() |

Роли — через ASP.NET Identity Roles: `User`, `Admin`.

---

### Category
Иерархический справочник категорий.

| Поле | Тип | Ограничения |
|------|-----|-------------|
| Id | Guid | PK |
| Name | string(80) | not null |
| Slug | string(100) | not null, unique |
| ParentId | Guid? | FK → Category.Id, nullable |
| SortOrder | int | default 0 |

- `ParentId == null` → корневая категория.
- `Slug` — для человекочитаемых URL (`/category/electronics`).
- Индекс: `unique(Slug)`, `index(ParentId)`.

---

### Listing (объявление)
Центральная сущность.

| Поле | Тип | Ограничения |
|------|-----|-------------|
| Id | Guid | PK |
| Title | string(120) | not null |
| Description | string(5000) | not null |
| Price | decimal(12,2) | >= 0 |
| Condition | enum (ListingCondition) | New / Used |
| Status | enum (ListingStatus) | Draft / Active / Sold / Archived |
| City | string(80) | not null |
| CategoryId | Guid | FK → Category.Id |
| UserId | Guid | FK → User.Id (владелец) |
| ViewsCount | int | default 0 |
| CreatedAt | DateTimeOffset | default now() |
| UpdatedAt | DateTimeOffset | default now() |

Индексы: `index(CategoryId)`, `index(UserId)`, `index(Status)`, `index(CreatedAt)`,
полнотекстовый по `Title`/`Description` (GIN + tsvector) — опционально на этапе поиска.

**Enums:**
```csharp
enum ListingCondition { New = 1, Used = 2 }
enum ListingStatus    { Draft = 1, Active = 2, Sold = 3, Archived = 4 }
```

---

### ListingImage
Фотографии объявления.

| Поле | Тип | Ограничения |
|------|-----|-------------|
| Id | Guid | PK |
| ListingId | Guid | FK → Listing.Id, cascade delete |
| Url | string(500) | not null |
| IsPrimary | bool | default false |
| SortOrder | int | default 0 |

- Одно фото на объявление помечено `IsPrimary = true` (обложка).
- Индекс: `index(ListingId)`.

---

### Favorite (избранное)
Связь многие-ко-многим User ↔ Listing.

| Поле | Тип | Ограничения |
|------|-----|-------------|
| Id | Guid | PK |
| UserId | Guid | FK → User.Id, cascade delete |
| ListingId | Guid | FK → Listing.Id, cascade delete |
| CreatedAt | DateTimeOffset | default now() |

- Уникальный составной индекс `unique(UserId, ListingId)` — нельзя добавить дважды.

---

### Conversation (диалог) — этап 2
Диалог покупателя и продавца по конкретному объявлению.

| Поле | Тип | Ограничения |
|------|-----|-------------|
| Id | Guid | PK |
| ListingId | Guid | FK → Listing.Id |
| BuyerId | Guid | FK → User.Id |
| SellerId | Guid | FK → User.Id |
| CreatedAt | DateTimeOffset | default now() |
| LastMessageAt | DateTimeOffset | для сортировки диалогов |

- Уникальный индекс `unique(ListingId, BuyerId)` — один диалог на пару покупатель+объявление.
- FK на User для Buyer/Seller — с `Restrict` (иначе конфликт множественных cascade-путей в PostgreSQL).

---

### Message (сообщение) — этап 2

| Поле | Тип | Ограничения |
|------|-----|-------------|
| Id | Guid | PK |
| ConversationId | Guid | FK → Conversation.Id, cascade delete |
| SenderId | Guid | FK → User.Id |
| Text | string(2000) | not null |
| IsRead | bool | default false |
| CreatedAt | DateTimeOffset | default now() |

- Индекс: `index(ConversationId, CreatedAt)`.

---

## 3. Каскады и целостность (важно для PostgreSQL/EF)

- `Listing` → `ListingImage`: **Cascade** (удалили объявление — удалились фото).
- `Listing` / `User` → `Favorite`: **Cascade**.
- `Conversation` → `Message`: **Cascade**.
- `Conversation.Buyer/Seller` → `User`: **Restrict** (избежать множественных cascade-путей).
- `Category.Parent` → `Category`: **Restrict** (нельзя удалить категорию с детьми).
- `Category` → `Listing`: **Restrict** (нельзя удалить категорию с объявлениями).

---

## 4. Seed-данные (начальное наполнение)

- Роли: `Admin`, `User`.
- Один admin-пользователь (из конфигурации/секретов).
- Дерево категорий, например:
  - Электроника → Телефоны, Ноутбуки, ТВ
  - Транспорт → Автомобили, Запчасти
  - Недвижимость → Квартиры, Дома
  - Личные вещи → Одежда, Обувь
  - Дом и сад → Мебель, Бытовая техника

---

## 5. Соответствие сущностей слоям Clean Architecture

| Слой | Что живёт |
|------|-----------|
| **Domain** | Сущности (Listing, Category, ...), enums, доменные исключения. Без зависимостей от EF. |
| **Application** | DTO, команды/запросы (CQRS + MediatR), интерфейсы репозиториев, валидаторы. |
| **Infrastructure** | EF Core `DbContext`, конфигурации сущностей (IEntityTypeConfiguration), миграции, Identity, реализация репозиториев, файловое хранилище. |
| **API** | Контроллеры, middleware, DI, Swagger, JWT-настройки. |

> Примечание: при Clean Architecture Identity обычно живёт в Infrastructure.
> `User` как доменная сущность может быть отдельной, а `ApplicationUser : IdentityUser<Guid>`
> — в Infrastructure. Для pet-проекта допустимо совместить, чтобы не плодить маппинг.
