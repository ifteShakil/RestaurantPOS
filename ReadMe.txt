1. Solution Architecture

RestaurantPOS.sln
├── RestaurantPOS.API            → ASP.NET Core Web API
├── RestaurantPOS.Core           → Entities, Interfaces (Repository/UnitOfWork)
├── RestaurantPOS.Infrastructure → DbContext, Repositories, Migrations
├── RestaurantPOS.Maui           → (এখনো শুরু হয়নি)
└── RestaurantPOS.Shared         → DTOs


2. Domain Models (Multi-branch ও International-ready)
Entity	মূল বৈশিষ্ট্য
Branch	CountryCode, CurrencyCode, TaxRatePercent, TimeZoneId, LocaleCode
MenuCategory / MenuItem / MenuItemVariant	কমন মেনু, ব্রাঞ্চভেদে override সাপোর্ট
MenuItemBranchPrice	প্রতি ব্রাঞ্চে আলাদা দাম/কারেন্সি/অ্যাভেইলেবিলিটি
RestaurantTable	BranchId, Status (Available/Occupied/...)
AppUser	Role-based (SuperAdmin/BranchManager/Cashier/Waiter/KitchenStaff), হ্যাশড পাসওয়ার্ড
Order / OrderItem	BranchId, দাম সার্ভার-সাইডে লক হয়
Bill	VAT স্ন্যাপশট, CurrencyCode
Payment / PaymentMethodType	Dynamic পেমেন্ট মেথড (দেশভেদে ভিন্ন হতে পারে — Cash, Card, MTN MoMo)


3. Data Access Layer
Generic Repository Pattern — IGenericRepository<T>
নির্দিষ্ট Repository — IOrderRepository, IMenuItemRepository, IBranchRepository (কাস্টম কোয়েরিসহ)
Unit of Work — সব রিপোজিটরি এক জায়গায়, একটাই SaveChangesAsync()


4. DTOs (Shared প্রজেক্ট)
Entity কখনো সরাসরি এক্সপোজ হয় না — প্রতিটার জন্য আলাদা DTO (Response ও Create/Update):
Branch, MenuCategory, MenuItem, Table, Order, Bill, Payment, User

5. Entity ↔ DTO ম্যাপিং 
AutoMapper দিয়ে AppMappingProfile.cs-এ কেন্দ্রীভূতভাবে সব ম্যাপিং রুল সংজ্ঞায়িত

6. API Controllers (সব টেস্ট করা হয়েছে ✅)
BranchController — CRUD + country filter
MenuCategoryController — CRUD
MenuController — CRUD + branch-aware pricing
TableController — CRUD + status update
UserController — CRUD + password hashing (PasswordHasher)
OrderController — Create (সার্ভার-সাইড price lookup), status update
BillController — Bill generation (VAT ক্যালকুলেশন) + Payment recording


7. Database
SQL Server, EF Core Migrations দিয়ে ম্যানেজ হচ্ছে
Seed data: PaymentMethodType (HasData() দিয়ে), Branch (Seeder ক্লাস দিয়ে — Eswatini Mbabane branch)


8. আন্তর্জাতিকীকরণ (Eswatini-কেন্দ্রিক)
Multi-currency (SZL সহ যেকোনো কারেন্সি)
Configurable VAT rate per branch (15% Eswatini স্ট্যান্ডার্ড)
Dynamic payment methods (MTN Mobile Money যোগ করা আছে)
Locale/timezone সাপোর্ট


##### এখনো বাকি যা আছে

1
JWT Authentication
এখন সব এন্ডপয়েন্ট ওপেন — কেউ যেকোনো Order/Payment দেখতে-বানাতে পারবে। JWT দিয়ে লগইন এন্ডপয়েন্ট এবং [Authorize] অ্যাট্রিবিউট যোগ করতে হবে।

2
Branch ডাটা আইসোলেশন
এক ব্রাঞ্চের Waiter যেন অন্য ব্রাঞ্চের Order দেখতে না পারে। EF Core Global Query Filter দিয়ে এটা অটোমেটিক করা যাবে।

3
Input Validation
সার্ভার-সাইড লজিক ঠিক আছে, কিন্তু এখনো DTO Validation Attribute (যেমন [Required], [Range]) যোগ করা হয়নি — এটা যোগ করা দরকার।

4
Global Exception Handling
এখন সব এন্ডপয়েন্টে সাধারণ exception হলে 500 error সম্পূর্ণ stack trace-সহ বেরিয়ে আসে। একটা middleware বানিয়ে clean, consistent error response তৈরি করা দরকার।

5
MAUI অ্যাপ স্ক্রিন বানানো
MAUI অ্যাপে লগইন, মেনু স্ক্রিন, অর্ডার স্ক্রিন বানিয়ে API-র সাথে কানেক্ট করা শুরু করতে হবে (HttpClient দিয়ে)।

6
SignalR (কিচেন রিয়েল-টাইম অর্ডার)
অর্ডার নেওয়ার সঙ্গে সঙ্গে রিয়েল-টাইমে রান্নাঘরে পাঠানোর জন্য SignalR সেটআপ করতে হবে।