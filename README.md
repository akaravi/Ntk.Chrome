# Ntk.Chrome - سیستم ورود محتوا با دسترسی DEveloperTools

## 📋 معرفی

**Ntk.Chrome** یک نرم‌افزار قدرتمند برای اتوماسیون وب و نظارت بر درخواست‌های شبکه است که با استفاده از Selenium WebDriver و Chrome DevTools Protocol ساخته شده است. این نرم‌افزار امکان پر کردن خودکار فرم‌های وب، نظارت بر درخواست‌های HTTP، و تحلیل ترافیک شبکه را فراهم می‌کند.

### 🔧 اتوماسیون وب
- **پر کردن خودکار فرم‌ها**: قابلیت پر کردن خودکار فیلدهای مختلف در وب‌سایت‌ها
- **پشتیبانی از انواع فیلد**: input، textarea، select و سایر عناصر فرم
- **تشخیص هوشمند فیلدها**: جستجوی فیلدها بر اساس name، id، placeholder و XPath
- **ارسال خودکار فرم**: کلیک خودکار روی دکمه‌های submit

### 🌐 نظارت بر شبکه
- **ردیابی درخواست‌های HTTP**: نظارت بر تمام درخواست‌ها و پاسخ‌های شبکه
- **نمایش جزئیات درخواست**: نمایش headers، body، status code و سایر اطلاعات
- **فیلتر کردن درخواست‌ها**: نمایش فقط درخواست‌های مهم (صفحات اصلی)
- **فرمت‌های مختلف نمایش**: JSON، HTML، و متن ساده

### 📊 نظارت پیشرفته بر URL
- **نظارت بر URL های خاص**: تنظیم URL های خاص برای نظارت
- **استخراج پارامترها**: استخراج خودکار پارامترهای مهم از پاسخ‌ها
- **هشدارهای هوشمند**: نمایش پاپ‌آپ و لاگ‌گیری برای رویدادهای مهم
- **ذخیره لاگ‌ها**: ذخیره اطلاعات نظارت در فایل‌های جداگانه

### 🔄 مدیریت نسخه‌ها
- **دانلود خودکار Chromium**: دانلود و نصب خودکار نسخه‌های سازگار
- **مدیریت ChromeDriver**: به‌روزرسانی خودکار ChromeDriver
- **سازگاری نسخه‌ها**: تضمین سازگاری بین Chromium و ChromeDriver
- **آرشیو نسخه‌ها**: نگهداری نسخه‌های قبلی برای استفاده مجدد

## 🛠️ پیش‌نیازها

### سیستم عامل
- **Windows 10/11** (64-bit)
- **.NET 9.0 Runtime** یا بالاتر

### نرم‌افزارهای مورد نیاز
- **Google Chrome** (اختیاری - در صورت عدم وجود Chromium دانلود شده)
- **Visual Studio 2022** یا **Visual Studio Code** (برای توسعه)

### پکیج‌های NuGet
```
Selenium.WebDriver (4.16.2)
Selenium.Support (4.16.2)
Selenium.WebDriver.ChromeDriver (141.0.7390.7800)
Newtonsoft.Json (13.0.4)
Microsoft.Extensions.Configuration.Json (9.0.10)
Microsoft.Extensions.DependencyInjection (9.0.10)
Serilog (4.3.0)
Serilog.Sinks.File (7.0.0)
```

## 📦 نصب و راه‌اندازی

### 1. کلون کردن پروژه
```bash
git clone https://github.com/your-username/Ntk.Chrome.git
cd Ntk.Chrome
```

### 2. بازگردانی پکیج‌ها
```bash
dotnet restore
```

### 3. کامپایل پروژه
```bash
dotnet build --configuration Release
```

### 4. اجرای برنامه
```bash
dotnet run
```

## 🚀 راهنمای استفاده

### اولین اجرا

1. **اجرای برنامه**: فایل `Ntk.Chrome.exe` را اجرا کنید
2. **تنظیمات اولیه**: در اولین اجرا، فرم تنظیمات نمایش داده می‌شود
3. **پیکربندی مسیرها**: مسیرهای ChromeDriver، برنامه و لاگ‌ها را تنظیم کنید
4. **تنظیم نسخه‌ها**: نسخه‌های Chromium و ChromeDriver مورد نظر را وارد کنید

### تنظیمات برنامه

#### تنظیمات اصلی (settings.json)
```json
{
  "ChromeDriverPath": "ChromeDriver",
  "ProgramPath": "C:\\Program Files\\Ntk.Chrome",
  "LogPath": "%TEMP%\\NtkChromeDriver\\logs",
  "EnableProgramStatus": true,
  "ChromiumVersion": "120.0.6099.109",
  "ChromeDriverVersion": "120.0.6099.109"
}
```

#### تنظیمات وب‌سایت (website_settings.json)
```json
{
  "WebsiteUrl": "https://example.com/login",
  "Fields": [
    {
      "Name": "username",
      "Value": "your_username"
    },
    {
      "Name": "password", 
      "Value": "your_password"
    }
  ],
  "UrlMonitoring": [
    {
      "Name": "Login Success",
      "Url": "/dashboard",
      "Parameters": [
        {
          "Name": "user_id",
          "Description": "User ID after login",
          "Required": true
        }
      ],
      "ShowPopup": true,
      "LogToFile": true,
      "PopupTitle": "Login Successful"
    }
  ]
}
```

### استفاده از برنامه

#### 1. راه‌اندازی اولیه
- برنامه را اجرا کنید
- منتظر بمانید تا راه‌اندازی اولیه تکمیل شود
- در صورت نیاز، Chromium و ChromeDriver دانلود خواهند شد

#### 2. شروع اتوماسیون
- روی دکمه **"شروع"** کلیک کنید
- برنامه Chrome را باز کرده و به وب‌سایت مورد نظر می‌رود
- فرم‌ها به صورت خودکار پر می‌شوند
- درخواست‌های شبکه در جدول نمایش داده می‌شوند

#### 3. نظارت بر درخواست‌ها
- درخواست‌های شبکه در جدول نمایش داده می‌شوند
- برای مشاهده جزئیات، روی درخواست مورد نظر کلیک کنید
- فرمت نمایش را انتخاب کنید (متن، JSON، HTML)

#### 4. توقف برنامه
- روی دکمه **"توقف"** کلیک کنید
- ChromeDriver متوقف شده و منابع آزاد می‌شوند

## 📁 ساختار پروژه

```
Ntk.Chrome/
├── Forms/                          # فرم‌های رابط کاربری
│   ├── MainForm.cs                 # فرم اصلی برنامه
│   ├── SettingsForm.cs             # فرم تنظیمات
│   ├── WebsiteSettingsForm.cs      # فرم تنظیمات وب‌سایت
│   └── UrlMonitoringConfigForm.cs  # فرم تنظیمات نظارت
├── Services/                       # سرویس‌های برنامه
│   ├── ChromeDriverService.cs      # سرویس مدیریت ChromeDriver
│   ├── NetworkService.cs           # سرویس نظارت بر شبکه
│   ├── LoggingService.cs           # سرویس لاگ‌گیری
│   └── VersionManager.cs           # مدیریت نسخه‌ها
├── Models/                         # مدل‌های داده
│   └── AppSettings.cs              # تنظیمات برنامه
├── checkjs/                        # فایل‌های JavaScript
├── logs/                           # فایل‌های لاگ
├── Program.cs                      # نقطه ورود برنامه
└── Ntk.Chrome.csproj              # فایل پروژه
```

## 🔧 تنظیمات پیشرفته

### تنظیمات ChromeDriver
```csharp
var options = new ChromeOptions();
options.AddArgument("--no-sandbox");
options.AddArgument("--disable-dev-shm-usage");
options.AddArgument("--disable-gpu");
options.AddArgument("--remote-debugging-port=0");
```

### تنظیمات نظارت بر URL
- **نام**: نام توصیفی برای تنظیمات نظارت
- **URL**: بخشی از URL که باید نظارت شود
- **پارامترها**: فهرست پارامترهایی که باید استخراج شوند
- **نمایش پاپ‌آپ**: نمایش هشدار در صورت وقوع رویداد
- **ذخیره در فایل**: ذخیره اطلاعات در فایل لاگ

### تنظیمات لاگ‌گیری
- **مسیر لاگ‌ها**: مسیر ذخیره فایل‌های لاگ
- **سطح لاگ**: تنظیم سطح جزئیات لاگ‌ها
- **چرخش لاگ‌ها**: مدیریت اندازه فایل‌های لاگ

## 🐛 عیب‌یابی

### مشکلات رایج

#### 1. خطا در دانلود Chromium
```
خطا: خطا در دانلود Chromium
راه‌حل: بررسی اتصال اینترنت و مسیرهای فایروال
```

#### 2. عدم سازگاری نسخه‌ها
```
خطا: نسخه ChromeDriver با Chromium سازگار نیست
راه‌حل: به‌روزرسانی نسخه‌ها در تنظیمات
```

#### 3. خطا در پیدا کردن فیلدها
```
خطا: فیلد مورد نظر یافت نشد
راه‌حل: بررسی نام فیلدها در تنظیمات وب‌سایت
```

### بررسی لاگ‌ها
- فایل‌های لاگ در پوشه `logs/` ذخیره می‌شوند
- لاگ‌های نظارت در `logs/url-monitoring/` قرار دارند
- لاگ‌های ChromeDriver در مسیر ChromeDriver ذخیره می‌شوند

## 📈 عملکرد و بهینه‌سازی

### تنظیمات عملکرد
- **Timeout**: تنظیم زمان انتظار برای عملیات
- **Retry Logic**: منطق تلاش مجدد برای عملیات ناموفق
- **Memory Management**: مدیریت حافظه برای عملیات طولانی

### بهینه‌سازی شبکه
- **فیلتر درخواست‌ها**: نمایش فقط درخواست‌های مهم
- **کاهش حجم لاگ**: عدم ذخیره درخواست‌های غیرضروری
- **مدیریت Thread**: استفاده بهینه از Thread ها

## 🔒 امنیت

### نکات امنیتی
- **ذخیره امن اطلاعات**: رمزگذاری اطلاعات حساس
- **مدیریت Session**: مدیریت صحیح session ها
- **اعتبارسنجی ورودی**: بررسی صحت اطلاعات ورودی

### بهترین روش‌ها
- استفاده از متغیرهای محیطی برای اطلاعات حساس
- محدود کردن دسترسی به فایل‌های تنظیمات
- استفاده از HTTPS برای ارتباطات


### گزارش باگ
- از طریق Issues در GitHub
- ارائه جزئیات کامل خطا
- ضمیمه کردن فایل‌های لاگ

### پیشنهاد ویژگی
- توضیح کامل نیاز
- ارائه مثال‌های کاربردی
- بررسی امکان‌پذیری

## 📄 مجوز

این پروژه تحت مجوز MIT منتشر شده است. برای جزئیات بیشتر، فایل LICENSE را مطالعه کنید.


### راه‌های ارتباطی
- **GitHub Issues**: برای گزارش مشکلات
- **Email**: info@alikaravi.com 
- **تلفن**: (00971) 504504324
- **وب‌سایت**: [alikaravi.com](https://alikaravi.com)

### منابع م1فید
- [Selenium WebDriver Documentation](https://selenium-python.readthedocs.io/)
- [Chrome DevTools Protocol](https://chromedevtools.github.io/devtools-protocol/)
- [.NET 9.0 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Alireza Karavi Portfolio](https://alikaravi.com)

---

**توجه**: این نرم‌افزار برای اهداف آموزشی و توسعه ساخته شده است. لطفاً از آن مطابق با قوانین و مقررات استفاده کنید.

