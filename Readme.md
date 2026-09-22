# 🚀 Order Service

Modern .NET 10 ve Clean Architecture prensipleriyle geliştirilmiş, JWT tabanlı kimlik doğrulama, PostgreSQL, Redis, RabbitMQ ve MassTransit kullanan mikroservis tabanlı örnek bir backend projesidir.

Bu proje, mezuniyet sonrasında backend geliştirme becerilerimi sistematik olarak geliştirmek amacıyla yürüttüğüm **Backend Günlüğü** sürecinin bir parçasıdır.

Geliştirme sürecinde gerçek backend task mantığı takip edilmiş; kod, testler, Docker altyapısı, CI pipeline ve öğrenme süreci GitHub ve Medium üzerinden belgelenmiştir.

---

## 📌 Proje Hakkında

Proje iki bağımsız servisten oluşmaktadır:

- **Order Service** → Siparişlerin oluşturulması, güncellenmesi, listelenmesi ve yönetilmesinden sorumludur.
- **Notification Service** → Order Service tarafından yayınlanan `OrderCreatedEvent` olayını RabbitMQ üzerinden tüketerek bildirim kaydı oluşturur.

Servisler arasında doğrudan HTTP bağımlılığı bulunmaz.

```text
┌─────────────────────┐
│    Order Service    │
│       :8080         │
│                     │
│ Clean Architecture  │
│ JWT Authentication  │
│ PostgreSQL          │
│ Redis Cache         │
└──────────┬──────────┘
           │
           │ OrderCreatedEvent
           ▼
┌─────────────────────┐
│      RabbitMQ       │
│       :5672         │
└──────────┬──────────┘
           │
           │ Consume
           ▼
┌─────────────────────┐
│ NotificationService │
│       :8081         │
│                     │
│ MassTransit Consumer│
│ PostgreSQL          │
└─────────────────────┘
Bu yapı sayesinde servisler gevşek bağlı (loosely coupled) şekilde çalışır.
🏗️ Mimari
Order Service
src
├── OrderService.API
├── OrderService.Application
├── OrderService.Domain
└── OrderService.Infrastructure

Katmanların sorumlulukları:

Katman	Sorumluluk
API	HTTP istekleri, authentication ve endpoint'ler
Application	Use Case'ler, CQRS, handler'lar, DTO'lar ve abstraction'lar
Domain	Entity'ler ve iş kuralları
Infrastructure	EF Core, PostgreSQL, Redis, RabbitMQ ve dış servis implementasyonları
Notification Service
src
├── NotificationService.API
├── NotificationService.Application
├── NotificationService.Domain
└── NotificationService.Infrastructure

Notification Service, OrderCreatedEvent mesajını MassTransit üzerinden tüketir ve kendi PostgreSQL veritabanına notification kaydı oluşturur.

🛠️ Kullanılan Teknolojiler
Backend
.NET 10
C#
ASP.NET Core Minimal API
Clean Architecture
CQRS
Data
Entity Framework Core
PostgreSQL
Redis
Authentication & Authorization
JWT Bearer Authentication
Roles & Claims
Policy-based Authorization
Messaging & Microservices
RabbitMQ
MassTransit
Event-driven communication
Testing
xUnit
Moq
Integration Tests
Health Check Tests
Notification Consumer Tests
DevOps
Docker
Docker Compose
GitHub Actions
CI pipeline
Logging & Monitoring
Serilog
Health Checks
✨ Temel Özellikler
Order Management
Sipariş oluşturma
Sipariş listeleme
Sayfalama
Sipariş detayına erişim
Sipariş güncelleme
Sipariş silme
Kullanıcıya ait siparişleri görüntüleme
Authentication & Authorization
JWT token authentication
Role-based authorization
Admin / User / Customer policy'leri
Korumalı endpoint'ler
Validation
FluentValidation ile request validation
Exception Handling
Global Exception Middleware
Merkezi hata yönetimi
Logging
Serilog ile yapılandırılmış loglama
Console ve file logging
Caching
Redis tabanlı cache
Sayfalı sipariş listelerinde caching
Create / Update / Delete işlemlerinde cache invalidation
Optimistic Concurrency

Sipariş güncellemelerinde optimistic concurrency yaklaşımı kullanılmıştır.

PostgreSQL tarafındaki version mekanizması ile aynı kaynağın eşzamanlı olarak değiştirilmesi kontrol edilir.

Event-Driven Communication

Sipariş oluşturulduğunda:

OrderService
    ↓
OrderCreatedEvent
    ↓
RabbitMQ
    ↓
NotificationService
    ↓
Notification DB

Notification Service, Order Service'i doğrudan çağırmadan event üzerinden bildirim oluşturur.

🐳 Docker Compose

Projenin altyapısı Docker Compose ile birlikte çalışacak şekilde yapılandırılmıştır.

Çalışan temel servisler:

Servis	Port
Order Service	8080
Notification Service	8081
PostgreSQL	5432
Redis	6379
RabbitMQ	5672
RabbitMQ Management	15672

Sistemi başlatmak için:

docker compose up -d

Compose, veritabanı ve broker bilgilerini environment variable'lardan alır. Başlatmadan önce örnek değerleri kendi ortamınız için ayarlayın:

```powershell
$env:POSTGRES_PASSWORD = "your-local-password"
$env:JWT_SECRET = "your-long-random-jwt-secret"
$env:RABBITMQ_USER = "your-rabbitmq-user"
$env:RABBITMQ_PASSWORD = "your-rabbitmq-password"
docker compose up -d --build
```

Çalışan container'ları görüntülemek için:

docker compose ps

Sistemi durdurmak için:

docker compose down

PostgreSQL verilerinin bulunduğu volume'u silmek istemiyorsanız docker compose down -v kullanmayın.

▶️ Projeyi Çalıştırma
Gereksinimler
.NET 10 SDK
Docker Desktop
Git

Projeyi klonlayın:

git clone https://github.com/elifasik1/Order-Service.git
cd Order-Service

Dependency'leri yükleyin:

dotnet restore

Projeyi build edin:

dotnet build

Docker ortamını başlatın:

docker compose up -d
🌐 API
Order Service
http://localhost:8080

Swagger:

http://localhost:8080/swagger

Health Check:

GET /health
Notification Service
http://localhost:8081

Swagger:

http://localhost:8081/swagger
🐇 RabbitMQ Management

RabbitMQ yönetim paneli:

http://localhost:15672

RabbitMQ credentials are supplied through `RABBITMQ_USER` and `RABBITMQ_PASSWORD`.

RabbitMQ üzerinden servislerin event tabanlı iletişimi ve consumer topolojisi gözlemlenebilir.

🧪 Testler

Projede unit ve integration testleri bulunmaktadır.

Tüm testleri çalıştırmak için:

dotnet test

Test kapsamı içerisinde:

Application handler testleri
Authentication testleri
Middleware testleri
Integration endpoint testleri
Health check testleri
Notification consumer testleri
Redis cache davranışı için mocked test senaryoları

bulunmaktadır.

🔄 CI Pipeline

GitHub Actions kullanılarak repository'ye yapılan push ve pull request işlemlerinde otomatik build ve test çalıştırılmaktadır.

Pipeline temel olarak:

Checkout
   ↓
Setup .NET
   ↓
Restore
   ↓
Build
   ↓
Database Setup
   ↓
Test

adımlarını takip eder.

📐 Architecture Decisions
Neden Clean Architecture?

Business logic'in framework ve infrastructure detaylarından ayrılması, test edilebilirliğin artırılması ve bağımlılıkların kontrol altında tutulması amacıyla Clean Architecture kullanılmıştır.

Neden RabbitMQ?

Order Service ile Notification Service arasında doğrudan HTTP bağımlılığı oluşturmak yerine asynchronous communication tercih edilmiştir.

Böylece Notification Service, Order Service'in implementation detaylarına bağımlı olmadan OrderCreatedEvent üzerinden çalışır.

Neden MassTransit?

RabbitMQ ile mesajlaşma altyapısını daha yüksek seviyede yönetmek, consumer ve publishing işlemlerini daha standart bir şekilde gerçekleştirmek amacıyla MassTransit kullanılmıştır.

Neden Redis?

Sık erişilen sayfalı sipariş listelerindeki tekrar eden database sorgularını azaltmak amacıyla caching uygulanmıştır.

Neden Optimistic Concurrency?

Aynı siparişin eşzamanlı olarak güncellenmesi durumunda son yazanın sessizce önceki değişikliği ezmesini önlemek amacıyla optimistic concurrency uygulanmıştır.

📊 Proje Akışı

Örnek bir sipariş oluşturma akışı:

Client
  │
  ▼
Order Service
  │
  ├── Validate Request
  │
  ├── Create Order
  │
  ├── Save to PostgreSQL
  │
  ├── Invalidate Redis Cache
  │
  └── Publish OrderCreatedEvent
              │
              ▼
          RabbitMQ
              │
              ▼
      Notification Service
              │
              ▼
       Notification DB

Buradaki önemli nokta:

Order Service ─X→ HTTP → Notification Service

yerine:

Order Service → RabbitMQ → Notification Service

kullanılmasıdır.

📚 Backend Günlüğü

Bu proje boyunca öğrendiğim konuları Medium'daki Backend Günlüğü serisinde belgeledim.

Bölüm	Konu
#1	Yapay Zekâ ile Değil, Yapay Zekâyla Öğreniyorum
#2	Yazılımda "Neden?" Sorusunu Öğreniyorum
#3	Bir API Yazmaktan, Bir Sistem Kurmaya
#4	Kod Çalışıyordu, Peki Ya Sonra?
#5	Bir Backend Ne Zaman Güvenilir Olur?
#6	Koduma Ne Kadar Güvenebilirim?
🎯 Projenin Amacı

Bu proje yalnızca çalışan bir API geliştirmek amacıyla oluşturulmamıştır.

Ana hedefler:

Backend geliştirme pratiğini gerçek task mantığıyla ilerletmek
Clean Architecture uygulamak
REST API geliştirmek
Authentication ve Authorization kullanmak
PostgreSQL ve EF Core ile çalışmak
Redis caching uygulamak
RabbitMQ ve MassTransit ile event-driven communication kurmak
Mikroservisler arası loose coupling yaklaşımını uygulamak
Unit ve integration testleri geliştirmek
Docker ile containerization uygulamak
GitHub Actions ile CI pipeline oluşturmak
Geliştirme sürecini dokümante etmek
📌 Proje Durumu

Proje, temel backend geliştirme aşamasından mikroservis tabanlı çalışan bir sisteme kadar geliştirilmiştir.

Mevcut yapı:

✅ Clean Architecture
✅ CQRS
✅ CRUD
✅ PostgreSQL
✅ Entity Framework Core
✅ FluentValidation
✅ JWT Authentication
✅ Authorization & Policies
✅ Global Exception Middleware
✅ Serilog
✅ Pagination
✅ Optimistic Concurrency
✅ Health Checks
✅ Unit Tests
✅ Integration Tests
✅ GitHub Actions
✅ Docker Compose
✅ Redis Caching
✅ RabbitMQ
✅ MassTransit
✅ Notification Service
✅ Event-driven communication
👩‍💻 Geliştirici

Elif Aşık

Computer Engineering Graduate

GitHub:

https://github.com/elifasik1

⭐ Eğer projeyi incelemek veya geliştirme sürecini takip etmek isterseniz repository'ye göz atabilirsiniz.