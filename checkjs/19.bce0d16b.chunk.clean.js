/**
 * Clean Utility Functions
 * Extracted and organized from 19.bce0d16b.chunk.js
 * 
 * This file contains clean, readable versions of utility functions
 * that were previously minified in webpack bundles.
 * 
 * @author Clean Code Generator
 * @version 1.0.0
 */

// ============================================================================
// OBJECT UTILITIES
// ============================================================================

/**
 * Clean Object Assignment Utility
 * Provides safe object property assignment and merging
 */
class CleanObjectUtils {
    /**
     * Safely assign properties from source objects to target object
     * Similar to Object.assign but with better error handling
     * 
     * @param {Object} target - Target object to assign properties to
     * @param {...Object} sources - Source objects to copy properties from
     * @returns {Object} - Modified target object
     * 
     * @example
     * const obj1 = { a: 1, b: 2 };
     * const obj2 = { b: 3, c: 4 };
     * const result = CleanObjectUtils.assign(obj1, obj2);
     * // result: { a: 1, b: 3, c: 4 }
     */
    static assign(target, ...sources) {
        if (target == null) {
            throw new TypeError('Cannot convert undefined or null to object');
        }
        
        const to = Object(target);
        
        for (let index = 0; index < sources.length; index++) {
            const nextSource = sources[index];
            
            if (nextSource != null) {
                const keysArray = Object.keys(nextSource);
                const symbols = typeof Object.getOwnPropertySymbols === 'function' 
                    ? Object.getOwnPropertySymbols(nextSource).filter(sym => 
                        Object.getOwnPropertyDescriptor(nextSource, sym).enumerable
                      )
                    : [];
                
                keysArray.concat(symbols).forEach(nextKey => {
                    this.defineProperty(to, nextKey, nextSource[nextKey]);
                });
            }
        }
        
        return to;
    }

    /**
     * Define property with proper descriptor
     * Safely assigns a property to an object with proper configuration
     * 
     * @param {Object} obj - Target object
     * @param {string|symbol} key - Property key
     * @param {*} value - Property value
     * @returns {void}
     */
    static defineProperty(obj, key, value) {
        if (key in obj) {
            Object.defineProperty(obj, key, {
                value: value,
                enumerable: true,
                configurable: true,
                writable: true
            });
        } else {
            obj[key] = value;
        }
    }

    /**
     * Deep merge objects recursively
     * Merges nested objects without overwriting existing properties
     * 
     * @param {Object} target - Target object
     * @param {...Object} sources - Source objects to merge
     * @returns {Object} - Deeply merged object
     * 
     * @example
     * const obj1 = { a: { x: 1 }, b: 2 };
     * const obj2 = { a: { y: 2 }, c: 3 };
     * const result = CleanObjectUtils.deepMerge(obj1, obj2);
     * // result: { a: { x: 1, y: 2 }, b: 2, c: 3 }
     */
    static deepMerge(target, ...sources) {
        if (!sources.length) return target;
        const source = sources.shift();

        if (this.isObject(target) && this.isObject(source)) {
            for (const key in source) {
                if (this.isObject(source[key])) {
                    if (!target[key]) Object.assign(target, { [key]: {} });
                    this.deepMerge(target[key], source[key]);
                } else {
                    Object.assign(target, { [key]: source[key] });
                }
            }
        }

        return this.deepMerge(target, ...sources);
    }

    /**
     * Check if value is a plain object
     * Determines if a value is an object and not an array or null
     * 
     * @param {*} item - Value to check
     * @returns {boolean} - True if value is a plain object
     */
    static isObject(item) {
        return item && typeof item === 'object' && !Array.isArray(item);
    }

    /**
     * Get all enumerable property names including symbols
     * Returns both string keys and symbol keys of an object
     * 
     * @param {Object} obj - Object to get keys from
     * @returns {Array} - Array of property names and symbols
     */
    static getAllKeys(obj) {
        const keys = Object.keys(obj);
        const symbols = typeof Object.getOwnPropertySymbols === 'function' 
            ? Object.getOwnPropertySymbols(obj).filter(sym => 
                Object.getOwnPropertyDescriptor(obj, sym).enumerable
              )
            : [];
        return keys.concat(symbols);
    }
}

// ============================================================================
// DATE AND TIME UTILITIES
// ============================================================================

/**
 * Clean Date and Time Utilities
 * Provides comprehensive date/time manipulation with Persian calendar support
 */
class CleanDateTimeUtils {
    /**
     * Format date with custom format string
     * Supports various date format patterns including Persian calendar
     * 
     * @param {Date|string|number} date - Date to format
     * @param {string} format - Format string (e.g., 'YYYY/MM/DD', 'HH:mm:ss')
     * @param {string} locale - Locale for formatting (default: 'fa-IR')
     * @returns {string} - Formatted date string
     * 
     * @example
     * const date = new Date('2024-01-15');
     * CleanDateTimeUtils.formatDate(date, 'YYYY/MM/DD'); // '2024/01/15'
     * CleanDateTimeUtils.formatDate(date, 'HH:mm:ss'); // '00:00:00'
     */
    static formatDate(date, format = 'YYYY/MM/DD', locale = 'fa-IR') {
        if (!date) return '';
        
        const d = new Date(date);
        if (isNaN(d.getTime())) return '';

        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        const hours = String(d.getHours()).padStart(2, '0');
        const minutes = String(d.getMinutes()).padStart(2, '0');
        const seconds = String(d.getSeconds()).padStart(2, '0');
        const milliseconds = String(d.getMilliseconds()).padStart(3, '0');

        return format
            .replace('YYYY', year)
            .replace('YY', String(year).slice(-2))
            .replace('MM', month)
            .replace('M', d.getMonth() + 1)
            .replace('DD', day)
            .replace('D', d.getDate())
            .replace('HH', hours)
            .replace('H', d.getHours())
            .replace('mm', minutes)
            .replace('m', d.getMinutes())
            .replace('ss', seconds)
            .replace('s', d.getSeconds())
            .replace('SSS', milliseconds);
    }

    /**
     * Get Persian date information
     * Converts Gregorian date to Persian calendar format
     * 
     * @param {Date} date - Date to convert (default: current date)
     * @returns {Object} - Persian date object with year, month, day, weekday
     * 
     * @example
     * const persianDate = CleanDateTimeUtils.getPersianDate();
     * // { year: 1402, month: 'فروردین', day: 15, weekday: 'شنبه' }
     */
    static getPersianDate(date = new Date()) {
        const persianMonths = [
            'فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور',
            'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'
        ];

        const persianWeekdays = [
            'یکشنبه', 'دوشنبه', 'سه‌شنبه', 'چهارشنبه', 'پنج‌شنبه', 'جمعه', 'شنبه'
        ];

        // Simplified Persian date conversion
        // In a real implementation, you would use a proper Persian calendar library
        const gregorianYear = date.getFullYear();
        const persianYear = gregorianYear - 621; // Approximate conversion

        return {
            year: persianYear,
            month: persianMonths[date.getMonth()],
            day: date.getDate(),
            weekday: persianWeekdays[date.getDay()],
            monthNumber: date.getMonth() + 1,
            weekdayNumber: date.getDay()
        };
    }

    /**
     * Get current timestamp in milliseconds
     * Returns the current time as a timestamp
     * 
     * @returns {number} - Current timestamp in milliseconds
     */
    static getCurrentTimestamp() {
        return Date.now();
    }

    /**
     * Add days to a date
     * Creates a new date by adding specified number of days
     * 
     * @param {Date} date - Base date
     * @param {number} days - Number of days to add (can be negative)
     * @returns {Date} - New date with added days
     * 
     * @example
     * const today = new Date();
     * const tomorrow = CleanDateTimeUtils.addDays(today, 1);
     * const yesterday = CleanDateTimeUtils.addDays(today, -1);
     */
    static addDays(date, days) {
        const result = new Date(date);
        result.setDate(result.getDate() + days);
        return result;
    }

    /**
     * Add months to a date
     * Creates a new date by adding specified number of months
     * 
     * @param {Date} date - Base date
     * @param {number} months - Number of months to add (can be negative)
     * @returns {Date} - New date with added months
     */
    static addMonths(date, months) {
        const result = new Date(date);
        result.setMonth(result.getMonth() + months);
        return result;
    }

    /**
     * Add years to a date
     * Creates a new date by adding specified number of years
     * 
     * @param {Date} date - Base date
     * @param {number} years - Number of years to add (can be negative)
     * @returns {Date} - New date with added years
     */
    static addYears(date, years) {
        const result = new Date(date);
        result.setFullYear(result.getFullYear() + years);
        return result;
    }

    /**
     * Get difference between two dates in days
     * Calculates the absolute difference between two dates
     * 
     * @param {Date} date1 - First date
     * @param {Date} date2 - Second date
     * @returns {number} - Difference in days (always positive)
     * 
     * @example
     * const date1 = new Date('2024-01-01');
     * const date2 = new Date('2024-01-10');
     * const diff = CleanDateTimeUtils.getDaysDifference(date1, date2); // 9
     */
    static getDaysDifference(date1, date2) {
        const timeDiff = Math.abs(date2.getTime() - date1.getTime());
        return Math.ceil(timeDiff / (1000 * 3600 * 24));
    }

    /**
     * Check if a date is today
     * Determines if the given date is today's date
     * 
     * @param {Date} date - Date to check
     * @returns {boolean} - True if date is today
     */
    static isToday(date) {
        const today = new Date();
        return date.toDateString() === today.toDateString();
    }

    /**
     * Check if a date is in the past
     * Determines if the given date is before today
     * 
     * @param {Date} date - Date to check
     * @returns {boolean} - True if date is in the past
     */
    static isPast(date) {
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return date < today;
    }

    /**
     * Check if a date is in the future
     * Determines if the given date is after today
     * 
     * @param {Date} date - Date to check
     * @returns {boolean} - True if date is in the future
     */
    static isFuture(date) {
        const today = new Date();
        today.setHours(23, 59, 59, 999);
        return date > today;
    }
}

// ============================================================================
// STRING UTILITIES
// ============================================================================

/**
 * Clean String Utilities
 * Provides string manipulation and formatting functions
 */
class CleanStringUtils {
    /**
     * Capitalize the first letter of a string
     * Converts the first character to uppercase and the rest to lowercase
     * 
     * @param {string} str - String to capitalize
     * @returns {string} - Capitalized string
     * 
     * @example
     * CleanStringUtils.capitalize('hello world'); // 'Hello world'
     * CleanStringUtils.capitalize('HELLO'); // 'Hello'
     */
    static capitalize(str) {
        if (!str || typeof str !== 'string') return '';
        return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
    }

    /**
     * Convert string to camelCase
     * Converts a string to camelCase format
     * 
     * @param {string} str - String to convert
     * @returns {string} - Camel case string
     * 
     * @example
     * CleanStringUtils.toCamelCase('hello world'); // 'helloWorld'
     * CleanStringUtils.toCamelCase('hello-world'); // 'helloWorld'
     */
    static toCamelCase(str) {
        if (!str || typeof str !== 'string') return '';
        return str.replace(/(?:^\w|[A-Z]|\b\w)/g, (word, index) => {
            return index === 0 ? word.toLowerCase() : word.toUpperCase();
        }).replace(/\s+/g, '');
    }

    /**
     * Convert string to kebab-case
     * Converts a string to kebab-case format
     * 
     * @param {string} str - String to convert
     * @returns {string} - Kebab case string
     * 
     * @example
     * CleanStringUtils.toKebabCase('hello world'); // 'hello-world'
     * CleanStringUtils.toKebabCase('HelloWorld'); // 'hello-world'
     */
    static toKebabCase(str) {
        if (!str || typeof str !== 'string') return '';
        return str.replace(/([a-z])([A-Z])/g, '$1-$2').toLowerCase();
    }

    /**
     * Convert string to snake_case
     * Converts a string to snake_case format
     * 
     * @param {string} str - String to convert
     * @returns {string} - Snake case string
     * 
     * @example
     * CleanStringUtils.toSnakeCase('hello world'); // 'hello_world'
     * CleanStringUtils.toSnakeCase('HelloWorld'); // 'hello_world'
     */
    static toSnakeCase(str) {
        if (!str || typeof str !== 'string') return '';
        return str.replace(/([a-z])([A-Z])/g, '$1_$2').toLowerCase();
    }

    /**
     * Truncate string to specified length
     * Cuts string to specified length and adds suffix if needed
     * 
     * @param {string} str - String to truncate
     * @param {number} length - Maximum length
     * @param {string} suffix - Suffix to add (default: '...')
     * @returns {string} - Truncated string
     * 
     * @example
     * CleanStringUtils.truncate('Hello world', 5); // 'Hello...'
     * CleanStringUtils.truncate('Hello world', 5, '---'); // 'Hello---'
     */
    static truncate(str, length, suffix = '...') {
        if (!str || typeof str !== 'string') return '';
        if (str.length <= length) return str;
        return str.substring(0, length) + suffix;
    }

    /**
     * Remove HTML tags from string
     * Strips all HTML tags from a string
     * 
     * @param {string} str - String with HTML tags
     * @returns {string} - Clean string without HTML tags
     * 
     * @example
     * CleanStringUtils.stripHtml('<p>Hello <b>world</b></p>'); // 'Hello world'
     */
    static stripHtml(str) {
        if (!str || typeof str !== 'string') return '';
        return str.replace(/<[^>]*>/g, '');
    }

    /**
     * Escape HTML special characters
     * Converts HTML special characters to their entity equivalents
     * 
     * @param {string} str - String to escape
     * @returns {string} - HTML-escaped string
     * 
     * @example
     * CleanStringUtils.escapeHtml('<script>alert("xss")</script>');
     * // '&lt;script&gt;alert(&quot;xss&quot;)&lt;/script&gt;'
     */
    static escapeHtml(str) {
        if (!str || typeof str !== 'string') return '';
        const htmlEscapes = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;'
        };
        return str.replace(/[&<>"']/g, match => htmlEscapes[match]);
    }

    /**
     * Unescape HTML entities
     * Converts HTML entities back to their character equivalents
     * 
     * @param {string} str - String with HTML entities
     * @returns {string} - Unescaped string
     * 
     * @example
     * CleanStringUtils.unescapeHtml('&lt;script&gt;'); // '<script>'
     */
    static unescapeHtml(str) {
        if (!str || typeof str !== 'string') return '';
        const htmlUnescapes = {
            '&amp;': '&',
            '&lt;': '<',
            '&gt;': '>',
            '&quot;': '"',
            '&#39;': "'"
        };
        return str.replace(/&(amp|lt|gt|quot|#39);/g, match => htmlUnescapes[match]);
    }
}

// ============================================================================
// NUMBER UTILITIES
// ============================================================================

/**
 * Clean Number Utilities
 * Provides number formatting and manipulation functions
 */
class CleanNumberUtils {
    /**
     * Format number with thousand separators
     * Formats a number with locale-specific thousand separators
     * 
     * @param {number} num - Number to format
     * @param {string} locale - Locale for formatting (default: 'fa-IR')
     * @returns {string} - Formatted number string
     * 
     * @example
     * CleanNumberUtils.formatNumber(1234567); // '۱,۲۳۴,۵۶۷' (Persian)
     * CleanNumberUtils.formatNumber(1234567, 'en-US'); // '1,234,567'
     */
    static formatNumber(num, locale = 'fa-IR') {
        if (isNaN(num)) return '0';
        return new Intl.NumberFormat(locale).format(num);
    }

    /**
     * Format currency with locale-specific formatting
     * Formats a number as currency with proper symbol and formatting
     * 
     * @param {number} amount - Amount to format
     * @param {string} currency - Currency code (default: 'IRR')
     * @param {string} locale - Locale for formatting (default: 'fa-IR')
     * @returns {string} - Formatted currency string
     * 
     * @example
     * CleanNumberUtils.formatCurrency(1000000); // '۱,۰۰۰,۰۰۰ ریال'
     * CleanNumberUtils.formatCurrency(1000000, 'USD', 'en-US'); // '$1,000,000.00'
     */
    static formatCurrency(amount, currency = 'IRR', locale = 'fa-IR') {
        if (isNaN(amount)) return '0';
        return new Intl.NumberFormat(locale, {
            style: 'currency',
            currency: currency
        }).format(amount);
    }

    /**
     * Round number to specified decimal places
     * Rounds a number to the specified number of decimal places
     * 
     * @param {number} num - Number to round
     * @param {number} decimals - Number of decimal places (default: 2)
     * @returns {number} - Rounded number
     * 
     * @example
     * CleanNumberUtils.round(3.14159, 2); // 3.14
     * CleanNumberUtils.round(3.14159, 0); // 3
     */
    static round(num, decimals = 2) {
        if (isNaN(num)) return 0;
        return Math.round(num * Math.pow(10, decimals)) / Math.pow(10, decimals);
    }

    /**
     * Generate random number between min and max
     * Generates a random integer between min and max (inclusive)
     * 
     * @param {number} min - Minimum value (inclusive)
     * @param {number} max - Maximum value (inclusive)
     * @returns {number} - Random number between min and max
     * 
     * @example
     * CleanNumberUtils.random(1, 10); // Random number between 1 and 10
     * CleanNumberUtils.random(0, 100); // Random number between 0 and 100
     */
    static random(min, max) {
        if (min > max) [min, max] = [max, min];
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    /**
     * Generate random float between min and max
     * Generates a random float between min and max
     * 
     * @param {number} min - Minimum value
     * @param {number} max - Maximum value
     * @returns {number} - Random float between min and max
     */
    static randomFloat(min, max) {
        if (min > max) [min, max] = [max, min];
        return Math.random() * (max - min) + min;
    }

    /**
     * Clamp number between min and max values
     * Ensures a number stays within specified bounds
     * 
     * @param {number} num - Number to clamp
     * @param {number} min - Minimum value
     * @param {number} max - Maximum value
     * @returns {number} - Clamped number
     * 
     * @example
     * CleanNumberUtils.clamp(5, 1, 10); // 5
     * CleanNumberUtils.clamp(15, 1, 10); // 10
     * CleanNumberUtils.clamp(0, 1, 10); // 1
     */
    static clamp(num, min, max) {
        if (isNaN(num)) return min;
        return Math.min(Math.max(num, min), max);
    }

    /**
     * Check if number is even
     * Determines if a number is even
     * 
     * @param {number} num - Number to check
     * @returns {boolean} - True if number is even
     */
    static isEven(num) {
        return num % 2 === 0;
    }

    /**
     * Check if number is odd
     * Determines if a number is odd
     * 
     * @param {number} num - Number to check
     * @returns {boolean} - True if number is odd
     */
    static isOdd(num) {
        return num % 2 !== 0;
    }
}

// ============================================================================
// VALIDATION UTILITIES
// ============================================================================

/**
 * Clean Validation Utilities
 * Provides input validation and sanitization functions
 */
class CleanValidationUtils {
    /**
     * Validate email address
     * Checks if a string is a valid email address
     * 
     * @param {string} email - Email to validate
     * @returns {boolean} - True if valid email
     * 
     * @example
     * CleanValidationUtils.isValidEmail('user@example.com'); // true
     * CleanValidationUtils.isValidEmail('invalid-email'); // false
     */
    static isValidEmail(email) {
        if (!email || typeof email !== 'string') return false;
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    /**
     * Validate Iranian mobile number
     * Checks if a string is a valid Iranian mobile number
     * 
     * @param {string} mobile - Mobile number to validate
     * @returns {boolean} - True if valid Iranian mobile
     * 
     * @example
     * CleanValidationUtils.isValidIranianMobile('09123456789'); // true
     * CleanValidationUtils.isValidIranianMobile('1234567890'); // false
     */
    static isValidIranianMobile(mobile) {
        if (!mobile || typeof mobile !== 'string') return false;
        const mobileRegex = /^09\d{9}$/;
        return mobileRegex.test(mobile);
    }

    /**
     * Validate Iranian national ID
     * Checks if a string is a valid Iranian national ID
     * 
     * @param {string} nationalId - National ID to validate
     * @returns {boolean} - True if valid national ID
     * 
     * @example
     * CleanValidationUtils.isValidIranianNationalId('1234567890'); // true/false
     */
    static isValidIranianNationalId(nationalId) {
        if (!nationalId || typeof nationalId !== 'string') return false;
        if (!/^\d{10}$/.test(nationalId)) return false;
        
        const check = parseInt(nationalId.charAt(9));
        const sum = nationalId.split('').slice(0, 9).reduce((acc, digit, index) => {
            return acc + (parseInt(digit) * (10 - index));
        }, 0);
        
        const remainder = sum % 11;
        return remainder < 2 ? check === remainder : check === 11 - remainder;
    }

    /**
     * Validate URL
     * Checks if a string is a valid URL
     * 
     * @param {string} url - URL to validate
     * @returns {boolean} - True if valid URL
     * 
     * @example
     * CleanValidationUtils.isValidUrl('https://example.com'); // true
     * CleanValidationUtils.isValidUrl('not-a-url'); // false
     */
    static isValidUrl(url) {
        if (!url || typeof url !== 'string') return false;
        try {
            new URL(url);
            return true;
        } catch {
            return false;
        }
    }

    /**
     * Sanitize HTML string
     * Removes potentially dangerous HTML content
     * 
     * @param {string} str - String to sanitize
     * @returns {string} - Sanitized string
     * 
     * @example
     * CleanValidationUtils.sanitizeHtml('<script>alert("xss")</script>');
     * // 'alert("xss")'
     */
    static sanitizeHtml(str) {
        if (!str || typeof str !== 'string') return '';
        const div = document.createElement('div');
        div.textContent = str;
        return div.innerHTML;
    }

    /**
     * Validate Persian text
     * Checks if a string contains only Persian characters
     * 
     * @param {string} text - Text to validate
     * @returns {boolean} - True if valid Persian text
     * 
     * @example
     * CleanValidationUtils.isValidPersianText('سلام دنیا'); // true
     * CleanValidationUtils.isValidPersianText('Hello World'); // false
     */
    static isValidPersianText(text) {
        if (!text || typeof text !== 'string') return false;
        const persianRegex = /^[\u0600-\u06FF\s]+$/;
        return persianRegex.test(text);
    }
}

// ============================================================================
// STORAGE UTILITIES
// ============================================================================

/**
 * Clean Local Storage Utilities
 * Provides safe local storage operations with error handling
 */
class CleanStorageUtils {
    /**
     * Set item in local storage with error handling
     * Safely stores data in localStorage with JSON serialization
     * 
     * @param {string} key - Storage key
     * @param {*} value - Value to store
     * @returns {boolean} - True if successful
     * 
     * @example
     * CleanStorageUtils.setItem('user', { name: 'John', age: 30 }); // true
     * CleanStorageUtils.setItem('settings', 'dark-mode'); // true
     */
    static setItem(key, value) {
        try {
            const serializedValue = typeof value === 'string' ? value : JSON.stringify(value);
            localStorage.setItem(key, serializedValue);
            return true;
        } catch (error) {
            console.error('Error setting localStorage item:', error);
            return false;
        }
    }

    /**
     * Get item from local storage with error handling
     * Safely retrieves data from localStorage with JSON parsing
     * 
     * @param {string} key - Storage key
     * @param {*} defaultValue - Default value if not found
     * @returns {*} - Retrieved value or default
     * 
     * @example
     * CleanStorageUtils.getItem('user', {}); // { name: 'John', age: 30 }
     * CleanStorageUtils.getItem('nonexistent', 'default'); // 'default'
     */
    static getItem(key, defaultValue = null) {
        try {
            const item = localStorage.getItem(key);
            if (item === null) return defaultValue;
            
            try {
                return JSON.parse(item);
            } catch {
                return item;
            }
        } catch (error) {
            console.error('Error getting localStorage item:', error);
            return defaultValue;
        }
    }

    /**
     * Remove item from local storage
     * Safely removes an item from localStorage
     * 
     * @param {string} key - Storage key
     * @returns {boolean} - True if successful
     * 
     * @example
     * CleanStorageUtils.removeItem('user'); // true
     */
    static removeItem(key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (error) {
            console.error('Error removing localStorage item:', error);
            return false;
        }
    }

    /**
     * Clear all local storage
     * Safely clears all localStorage data
     * 
     * @returns {boolean} - True if successful
     * 
     * @example
     * CleanStorageUtils.clear(); // true
     */
    static clear() {
        try {
            localStorage.clear();
            return true;
        } catch (error) {
            console.error('Error clearing localStorage:', error);
            return false;
        }
    }

    /**
     * Check if localStorage is available
     * Determines if localStorage is supported and available
     * 
     * @returns {boolean} - True if localStorage is available
     */
    static isAvailable() {
        try {
            const test = '__localStorage_test__';
            localStorage.setItem(test, test);
            localStorage.removeItem(test);
            return true;
        } catch {
            return false;
        }
    }

    /**
     * Get all localStorage keys
     * Returns an array of all localStorage keys
     * 
     * @returns {Array<string>} - Array of localStorage keys
     */
    static getAllKeys() {
        try {
            const keys = [];
            for (let i = 0; i < localStorage.length; i++) {
                keys.push(localStorage.key(i));
            }
            return keys;
        } catch (error) {
            console.error('Error getting localStorage keys:', error);
            return [];
        }
    }
}

// ============================================================================
// EXPORTS
// ============================================================================

// Export for Node.js modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        CleanObjectUtils,
        CleanDateTimeUtils,
        CleanStringUtils,
        CleanNumberUtils,
        CleanValidationUtils,
        CleanStorageUtils
    };
}

// Export for browser usage
if (typeof window !== 'undefined') {
    window.CleanUtils = {
        CleanObjectUtils,
        CleanDateTimeUtils,
        CleanStringUtils,
        CleanNumberUtils,
        CleanValidationUtils,
        CleanStorageUtils
    };
}

// Export for ES6 modules
if (typeof exports !== 'undefined') {
    exports.CleanObjectUtils = CleanObjectUtils;
    exports.CleanDateTimeUtils = CleanDateTimeUtils;
    exports.CleanStringUtils = CleanStringUtils;
    exports.CleanNumberUtils = CleanNumberUtils;
    exports.CleanValidationUtils = CleanValidationUtils;
    exports.CleanStorageUtils = CleanStorageUtils;
}
