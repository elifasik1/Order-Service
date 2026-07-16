# 🚀 Order Service

> Modern .NET ve Clean Architecture prensipleriyle geliştirdiğim örnek **Order Service** projesi.

Bu proje, mezuniyet sonrasında backend geliştirme becerilerimi sistematik olarak geliştirmek amacıyla başlattığım **"Backend Günlüğü"** serisinin bir parçasıdır.

Her görev, gerçek bir şirkette karşılaşılabilecek task mantığıyla ilerlemekte; geliştirme süreci GitHub commit'leri ve Medium yazılarıyla belgelenmektedir.

---

## 🏗️ Proje Mimarisi

Proje **Clean Architecture** yaklaşımıyla geliştirilmektedir.

```
src
├── OrderService.API
├── OrderService.Application
├── OrderService.Domain
└── OrderService.Infrastructure
```

Katmanlar tek sorumluluk prensibine göre ayrılmıştır.

- **API** → HTTP isteklerini karşılar.
- **Application** → Use Case / CQRS katmanı.
- **Domain** → Entity ve iş kuralları.
- **Infrastructure** → Veritabanı ve dış servisler.

---

## 🛠️ Kullanılan Teknolojiler

- .NET 10
- C#
- ASP.NET Core Minimal API
- Clean Architecture
- CQRS
- Git & GitHub

> Proje ilerledikçe aşağıdaki teknolojiler de eklenecektir:

- Entity Framework Core
- PostgreSQL
- FluentValidation
- JWT Authentication
- Docker
- Redis

---

# 📌 Sprint Durumu

## Sprint 1 ✅

- [x] TASK-001 — Clean Architecture kurulumu
- [x] TASK-002 — Health Endpoint
- [x] TASK-003 — Order Entity
- [x] TASK-004 — CreateOrder Request & Response DTO
- [x] TASK-005 — CreateOrder Handler

---

## 🚧 Devam Eden Geliştirmeler

- Validation
- Repository Pattern
- EF Core
- PostgreSQL
- Authentication
- Docker

---

# 📖 Backend Günlüğü

Bu proje boyunca öğrendiklerimi Medium'da düzenli olarak paylaşıyorum.

| Bölüm | Konu |
|--------|------|
| #1 | Yapay Zekâ ile Değil, Yapay Zekâyla Öğreniyorum *(Yakında)* |

> Yeni yazılar yayınlandıkça bu tablo güncellenecektir.

---

# 💻 Projeyi Çalıştırma

```bash
git clone https://github.com/elifasik1/Order-Service.git

cd Order-Service

dotnet restore

dotnet build

cd src/OrderService.API

dotnet run
```

API varsayılan olarak aşağıdaki adreste çalışacaktır.

```
http://localhost:5110
```

Health kontrolü:

```
GET /health
```

---

# 🎯 Projenin Amacı

Bu proje yalnızca çalışan bir Order Service geliştirmek için değil;

- Backend geliştirme pratiği yapmak,
- Clean Architecture prensiplerini uygulamak,
- Gerçek bir geliştirme sürecini deneyimlemek,
- Düzenli commit alışkanlığı kazanmak,
- Öğrenme sürecimi belgelemek

amacıyla geliştirilmektedir.

---

# 🤝 Geri Bildirim

Her türlü öneri ve geri bildirime açığım.

Backend geliştirme yolculuğum boyunca farklı bakış açılarıyla öğrenmeye devam etmek istiyorum.

---

⭐ Eğer projeyi ilgi çekici bulduysanız yıldız bırakmayı unutmayın.