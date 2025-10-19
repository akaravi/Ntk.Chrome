/**
 * Clean Login Form Component
 * Extracted and organized from 189.edfe8701.chunk.js
 * 
 * This module contains a comprehensive React login form component
 * with captcha, SMS verification, and password reset functionality.
 */

// ============================================================================
// CLEAN GENERATOR UTILITIES
// ============================================================================

/**
 * Clean Generator Function Factory
 * Creates generator functions with proper async/await support
 */
class CleanGeneratorFactory {
    constructor() {
        this.generator = this.createGenerator();
    }

    /**
     * Create generator function
     * @returns {Function} - Generator function
     */
    createGenerator() {
        const self = this;
        
        return function() {
            const generator = self.getGenerator();
            const { wrap, mark, awrap, AsyncIterator, async, keys, values } = generator;
            
            return {
                wrap: function(genFn, thisArg, _arguments, state) {
                    return self.wrap(genFn, thisArg, _arguments, state && state.reverse());
                },
                isGeneratorFunction: self.isGeneratorFunction,
                mark: mark,
                awrap: function(gen, skipTempReset) {
                    return new CleanAsyncGenerator(gen, skipTempReset);
                },
                AsyncIterator: AsyncIterator,
                async: function(genFn, thisArg, _arguments, state, info) {
                    return (self.isGeneratorFunction(genFn) ? self.async : function(genFn, thisArg, _arguments, state, info) {
                        const gen = self.async(genFn, thisArg, _arguments, state, info);
                        return gen.next().then(function(result) {
                            return result.done ? result.value : gen.next();
                        });
                    })(self.wrap(genFn), thisArg, _arguments, state, info);
                },
                keys: keys,
                values: values
            };
        };
    }

    /**
     * Get generator instance
     * @returns {Object} - Generator instance
     */
    getGenerator() {
        const self = this;
        
        return {
            wrap: function(genFn, thisArg, _arguments, state) {
                return self.wrap(genFn, thisArg, _arguments, state);
            },
            mark: self.mark,
            awrap: function(gen, skipTempReset) {
                return new CleanAsyncGenerator(gen, skipTempReset);
            },
            AsyncIterator: CleanAsyncIterator,
            async: self.async,
            keys: self.keys,
            values: self.values
        };
    }

    /**
     * Check if function is generator function
     * @param {Function} fn - Function to check
     * @returns {boolean} - True if generator function
     */
    isGeneratorFunction(fn) {
        const gen = this.getGenerator();
        const GeneratorFunction = (Object.getPrototypeOf ? Object.getPrototypeOf(gen) : gen.__proto__).constructor;
        
        return function(fn) {
            const constructor = typeof fn === "function" && fn.constructor;
            return !!constructor && (constructor === GeneratorFunction || "GeneratorFunction" === (constructor.displayName || constructor.name));
        }(fn);
    }

    /**
     * Wrap generator function
     * @param {Function} genFn - Generator function
     * @param {Object} thisArg - This argument
     * @param {Array} _arguments - Arguments
     * @param {Array} state - State
     * @returns {Function} - Wrapped function
     */
    wrap(genFn, thisArg, _arguments, state) {
        return this.mark(genFn).call(thisArg, _arguments, state);
    }

    /**
     * Mark function as generator
     * @param {Function} fn - Function to mark
     * @returns {Function} - Marked function
     */
    mark(fn) {
        return this.setGeneratorFunction(fn);
    }

    /**
     * Set generator function
     * @param {Function} fn - Function to set
     * @returns {Function} - Generator function
     */
    setGeneratorFunction(fn) {
        const self = this;
        
        return function() {
            return {
                wrap: function(genFn, thisArg, _arguments, state) {
                    return self.wrap(genFn, thisArg, _arguments, state);
                },
                isGeneratorFunction: self.isGeneratorFunction,
                mark: self.mark,
                awrap: function(gen, skipTempReset) {
                    return new CleanAsyncGenerator(gen, skipTempReset);
                },
                AsyncIterator: CleanAsyncIterator,
                async: self.async,
                keys: self.keys,
                values: self.values
            };
        };
    }

    /**
     * Create async iterator
     * @param {Function} genFn - Generator function
     * @param {Promise} promise - Promise
     * @returns {CleanAsyncIterator} - Async iterator
     */
    async(genFn, thisArg, _arguments, state, info) {
        return new CleanAsyncIterator(this.wrap(genFn), thisArg, _arguments, state, info);
    }

    /**
     * Get keys iterator
     * @param {Object} obj - Object to iterate
     * @returns {Function} - Keys iterator
     */
    keys(obj) {
        if (obj != null) {
            const iterator = obj[typeof Symbol !== "undefined" && Symbol.iterator || "@@iterator"];
            let index = 0;
            
            if (iterator) {
                return iterator.call(obj);
            }
            
            if (typeof obj.next === "function") {
                return obj;
            }
            
            if (!isNaN(obj.length)) {
                return {
                    next: function() {
                        if (obj && index >= obj.length) {
                            obj = undefined;
                        }
                        return {
                            value: obj && obj[index++],
                            done: !obj
                        };
                    }
                };
            }
        }
        
        throw new TypeError(typeof obj + " is not iterable");
    }

    /**
     * Get values iterator
     * @param {Object} obj - Object to iterate
     * @returns {Function} - Values iterator
     */
    values(obj) {
        const object = Object(obj);
        const keys = [];
        
        for (const key in object) {
            keys.unshift(key);
        }
        
        return function() {
            for (; keys.length;) {
                const key = keys.pop();
                if (key in object) {
                    return {
                        value: key,
                        done: false
                    };
                }
            }
            return {
                done: true
            };
        };
    }
}

/**
 * Clean Async Generator
 * Handles async generator operations
 */
class CleanAsyncGenerator {
    constructor(gen, skipTempReset) {
        this.v = gen;
        this.k = skipTempReset;
    }
}

/**
 * Clean Async Iterator
 * Handles async iteration
 */
class CleanAsyncIterator {
    constructor(gen, promise) {
        this.gen = gen;
        this.promise = promise;
    }

    /**
     * Invoke generator
     * @param {string} method - Method name
     * @param {*} arg - Argument
     * @param {Function} resolve - Resolve function
     * @param {Function} reject - Reject function
     * @returns {Promise} - Promise
     */
    _invoke(method, arg, resolve, reject) {
        const self = this;
        
        function invoke() {
            return new this.promise(function(resolve, reject) {
                self.invokeGenerator(method, arg, resolve, reject);
            });
        }
        
        return this.promise ? this.promise.then(invoke, invoke) : invoke();
    }

    /**
     * Invoke generator method
     * @param {string} method - Method name
     * @param {*} arg - Argument
     * @param {Function} resolve - Resolve function
     * @param {Function} reject - Reject function
     */
    invokeGenerator(method, arg, resolve, reject) {
        try {
            const result = this.gen[method](arg);
            const value = result.value;
            
            if (value instanceof CleanAsyncGenerator) {
                this.promise.resolve(value.v).then(function(result) {
                    this.invokeGenerator("next", result, resolve, reject);
                }.bind(this), function(error) {
                    this.invokeGenerator("throw", error, resolve, reject);
                }.bind(this));
            } else {
                this.promise.resolve(value).then(function(result) {
                    result.value = result;
                    resolve(result);
                }, function(error) {
                    this.invokeGenerator("throw", error, resolve, reject);
                }.bind(this));
            }
        } catch (error) {
            reject(error);
        }
    }
}

// ============================================================================
// CLEAN LOGIN FORM COMPONENT
// ============================================================================

/**
 * Clean Login Form Component
 * A comprehensive login form with captcha, SMS verification, and password reset functionality
 */
class CleanLoginForm extends React.Component {
    constructor(props) {
        super(props);
        
        this.state = {
            CapchaImg: "",
            capchaBg: "",
            smsButtinEnable: false,
            smsButtinDisplay: false,
            resetButtinEnable: false,
            resetButtinDisplay: false,
            userName: ""
        };
        
        this.generator = new CleanGeneratorFactory();
    }

    /**
     * Get captcha from server
     * @param {boolean} forceRefresh - Force refresh captcha
     */
    GetCapcha = this.generator.async(function*(forceRefresh) {
        const { loadingOff, loadingOn } = this.props;
        const { locale } = this.getCurrentLocale();
        
        if (forceRefresh) {
            loadingOn();
        }
        
        try {
            const data = { Lng: locale };
            const response = await this.callService({
                data: data,
                Root: "Sec",
                Path: 0
            });
            
            if (response) {
                this.setState({ CapchaImg: response.Data.ImgCapcha });
                
                if (response.Data.ImgCapcha.length === 0) {
                    this.saveToLocalStorage({
                        data: response.Data.ServerTime.toString().substring(
                            response.Data.ServerTime.toString().length - 4
                        ),
                        isObj: false,
                        saveLocal: true,
                        localName: "browser"
                    });
                }
                
                this.getCapchaBg();
            }
        } catch (error) {
            console.error('Error getting captcha:', error);
        } finally {
            if (forceRefresh) {
                loadingOff();
            }
        }
    });

    /**
     * Get captcha background
     */
    getCapchaBg = () => {
        this.setState({
            capchaBg: Math.floor(14 * Math.random()) + 1
        });
    };

    /**
     * Get all codes
     * @param {number} dateTime - Date time
     */
    GetAllCode = this.generator.async(function*(dateTime) {
        if (!this.isValidDateTime(dateTime)) {
            return;
        }
        
        const { loadingOff } = this.props;
        
        try {
            const response = await this.callService({
                Loading: false,
                Root: "Sec",
                Path: 3
            });
            
            if (response) {
                const data = {
                    list: response.Data,
                    dateTime: dateTime
                };
                this.saveToLocalStorage(data);
            }
        } catch (error) {
            console.error('Error getting all codes:', error);
        } finally {
            loadingOff();
        }
    });

    /**
     * Get all messages
     * @param {number} dateTime - Date time
     */
    GetAllMessage = this.generator.async(function*(dateTime) {
        if (!this.isValidMessageDateTime(dateTime)) {
            return;
        }
        
        try {
            const response = await this.callService({
                Loading: false,
                Root: "Sec",
                Path: 4
            });
            
            if (response) {
                const data = {
                    list: response.Data,
                    dateTime: dateTime
                };
                this.saveToLocalStorage(data);
            }
        } catch (error) {
            console.error('Error getting all messages:', error);
        }
    });

    /**
     * Handle username change
     * @param {Event} event - Input event
     */
    onChangeUserName = (event) => {
        const value = event.target.value;
        this.setState({ userName: value });
        
        if (this.isValidUsername({ Value: value, ShowNotify: false })) {
            this.setState({ smsButtinDisplay: true });
        } else {
            this.setState({ smsButtinDisplay: false });
        }
        
        const isAdmin = value.toLowerCase().startsWith("admin");
        this.setState({ resetButtinDisplay: isAdmin });
    };

    /**
     * Handle reset password click
     */
    handleResetPasswordClick = () => {
        const scus = document.getElementById("Scus").value.trim();
        
        if (scus) {
            localStorage.setItem("scus", scus);
        } else {
            localStorage.removeItem("scus");
        }
        
        this.props.history.push(this.getRoutePath(13));
    };

    /**
     * Submit form
     * @param {Event} event - Form event
     */
    submitform = this.generator.async(function*(event) {
        event.preventDefault();
        
        const scus = event.target.Scus.value.trim();
        const userName = event.target.UserName.value.trim();
        const password = event.target.Password.value;
        const capcha = event.target.Capcha ? event.target.Capcha.value : "notCapcha";
        const appVersion = localStorage.getItem("appVersion");
        
        // Validation
        if (!userName || userName.trim() === "" || !capcha || capcha.trim() === "" || !password || password.trim() === "") {
            this.showNotification("Please fill all required fields", "warning");
            return;
        }
        
        // Admin validation
        if (userName.toLowerCase() !== "admin" && (isNaN(userName) || userName.toLowerCase() === "admin") && scus === "") {
            this.showNotification("Invalid admin access", "error");
            return;
        }
        
        const { loadingOff, loadingOn, submitLoginData, setBasketCounter } = this.props;
        
        // Save captcha if exists
        if (this.state.CapchaImg.length > 0) {
            this.saveToLocalStorage({
                data: capcha,
                isObj: false,
                saveLocal: true,
                localName: "browser"
            });
        }
        
        const loginData = {
            Scus: scus,
            UserName: userName.toLowerCase(),
            Password: password,
            ClientVersion: appVersion,
            Lng: this.getCurrentLocale().locale
        };
        
        loadingOn();
        
        try {
            const response = await this.callService({
                data: loginData,
                Root: "Sec",
                Path: 1
            });
            
            if (response) {
                if (response.MessageCode) {
                    this.showNotification(this.getErrorMessage(response.MessageCode), "error");
                    
                    if (response.Data.ImgCapcha.length > 0) {
                        this.setState({ CapchaImg: response.Data.ImgCapcha });
                        this.getCapchaBg();
                    } else {
                        this.saveToLocalStorage({
                            data: response.Data.ServerTime.toString().substring(
                                response.Data.ServerTime.toString().length - 4
                            ),
                            isObj: false,
                            saveLocal: true,
                            localName: "browser"
                        });
                    }
                } else if (response.Data.status === 1) {
                    this.props.history.push("/ChangePass");
                    this.showNotification(response.MessageDes, "warning");
                } else if (response.Data.status === 2) {
                    setBasketCounter(response.Data.BasktCnt);
                    
                    const userData = Object.assign({}, response.Data, {
                        Key: this.getFromLocalStorage({
                            data: loginData.Password,
                            isObj: false,
                            saveLocal: false
                        })
                    });
                    
                    submitLoginData(userData);
                    this.props.history.push(this.getRoutePath(3));
                }
            }
        } catch (error) {
            console.error('Login error:', error);
            this.showNotification("Login failed", "error");
        } finally {
            loadingOff();
        }
    });

    /**
     * Send SMS
     */
    sendSms = this.generator.async(function*() {
        const userName = document.getElementById("UserName").value;
        
        if (!this.isValidUsername({ Value: userName, Title: this.getTranslation(570) })) {
            return;
        }
        
        const smsData = { MobileNumber: userName };
        
        try {
            const response = await this.callService({
                data: smsData,
                Root: "Sec",
                Path: 11
            });
            
            if (response) {
                this.showNotification(response.MessageDes, "success");
                this.setState({ smsButtinEnable: true });
                
                setTimeout(() => {
                    this.setState({ smsButtinEnable: false });
                }, 120000); // 2 minutes
            }
        } catch (error) {
            console.error('SMS error:', error);
        }
    });

    /**
     * Component did mount
     */
    componentDidMount = this.generator.async(function*() {
        if (this.isUserLoggedIn()) {
            this.props.history.push(this.getRoutePath(3));
            return;
        }
        
        const dateTime = parseInt(this.getCurrentDate().format("jYYYYjMMjDD") + this.getCurrentDate().format("HHmm"));
        
        yield this.GetCapcha(true);
        yield this.GetAllCode(dateTime);
        yield this.GetAllMessage(dateTime);
    });

    /**
     * Render component
     * @returns {JSX.Element} - Rendered component
     */
    render() {
        const { LoginForm } = this.props;
        const { CapchaImg } = this.state;
        
        return (
            <React.Fragment>
                {LoginForm.loading && <LoadingSpinner />}
                
                <a href="https://s.isaco.ir/27R" target="_blank" rel="noopener noreferrer">
                    <img 
                        src="https://s.isaco.ir/27Q" 
                        alt="" 
                        height={250} 
                        style={{ objectFit: "scale-down" }} 
                    />
                </a>
                
                <Form onSubmit={this.submitform.bind(this)} noValidate>
                    <div>
                        <FormTitle>
                            {this.getTranslation(29)} 
                        </FormTitle>
                        
                        <div>
                            <FormInput
                                name="Scus"
                                id="Scus"
                                label={`${this.getTranslation(28)}/${this.getTranslation(578)}`}
                                pattern="[0-9]+"
                                minLength={7}
                                maxLength={10}
                                dir="ltr"
                            />
                        </div>
                        
                        <div>
                            <FormInput
                                name="UserName"
                                id="UserName"
                                label={this.getTranslation(26)}
                                minLength={2}
                                required={true}
                                dir="ltr"
                                onChange={this.onChangeUserName}
                            />
                        </div>
                        
                        <div>
                            <FormInput
                                name="Password"
                                id="Password"
                                type="password"
                                label={this.getTranslation(27)}
                                required={true}
                                minLength={2}
                                dir="ltr"
                            />
                        </div>
                        
                        {CapchaImg.length > 0 && (
                            <React.Fragment>
                                <CaptchaContainer bg={this.state.capchaBg}>
                                    <div>
                                        <img src={CapchaImg} alt="captcha" />
                                    </div>
                                    <RefreshButton
                                        type="button"
                                        icon="refresh"
                                        onClick={() => this.GetCapcha(false)}
                                        title={this.getTranslation(538)}
                                    />
                                </CaptchaContainer>
                                
                                <div>
                                    <FormInput
                                        name="Capcha"
                                        id="Capcha"
                                        type="text"
                                        label={this.getTranslation(536)}
                                        required={true}
                                        pattern="[0-9]+"
                                        dir="ltr"
                                        autoComplete="off"
                                    />
                                </div>
                            </React.Fragment>
                        )}
                        
                        <SubmitButton
                            type="submit"
                            className="k-button"
                            value={this.getTranslation(5)}
                        />
                        
                        {this.state.smsButtinDisplay && (
                            <SMSButton
                                type="button"
                                className="k-button"
                                disabled={this.state.smsButtinEnable}
                                onClick={this.sendSms}
                                value={this.getTranslation(568)}
                            />
                        )}
                        
                        {this.state.resetButtinDisplay && (
                            <ResetButton
                                type="button"
                                className="k-button"
                                onClick={this.handleResetPasswordClick}
                                value={this.getTranslation(586)}
                            />
                        )}
                    </div>
                </Form>
                
                {this.getCurrentLocale().id === 1 && (
                    <SupplierLogo>
                        <img src={this.getSupplierLogo()} alt="" />
                    </SupplierLogo>
                )}
            </React.Fragment>
        );
    }

    // ============================================================================
    // UTILITY METHODS
    // ============================================================================

    /**
     * Get current locale
     * @returns {Object} - Current locale
     */
    getCurrentLocale() {
        return {
            locale: 'fa',
            id: 1,
            language: 'Persian'
        };
    }

    /**
     * Get current date
     * @returns {Object} - Current date
     */
    getCurrentDate() {
        return {
            format: (format) => {
                const now = new Date();
                const year = now.getFullYear();
                const month = String(now.getMonth() + 1).padStart(2, '0');
                const day = String(now.getDate()).padStart(2, '0');
                const hours = String(now.getHours()).padStart(2, '0');
                const minutes = String(now.getMinutes()).padStart(2, '0');
                
                return format
                    .replace('jYYYY', year)
                    .replace('jMM', month)
                    .replace('jDD', day)
                    .replace('HH', hours)
                    .replace('mm', minutes);
            }
        };
    }

    /**
     * Get translation
     * @param {string} key - Translation key
     * @returns {string} - Translated text
     */
    getTranslation(key) {
        const translations = {
            5: 'ورود',
            26: 'نام کاربری',
            27: 'رمز عبور',
            28: 'کد ملی',
            29: 'ورود به سیستم',
            138: 'خطا در ورود',
            536: 'کد امنیتی',
            537: 'لطفا کد امنیتی را وارد کنید',
            538: 'تازه کردن',
            568: 'ارسال پیامک',
            570: 'شماره موبایل',
            578: 'کد ملی',
            586: 'بازیابی رمز عبور'
        };
        
        return translations[key] || key;
    }

    /**
     * Get error message
     * @param {string} code - Error code
     * @returns {string} - Error message
     */
    getErrorMessage(code) {
        const errorMessages = {
            'INVALID_CREDENTIALS': 'نام کاربری یا رمز عبور اشتباه است',
            'CAPTCHA_REQUIRED': 'کد امنیتی الزامی است',
            'ACCOUNT_LOCKED': 'حساب کاربری قفل شده است'
        };
        
        return errorMessages[code] || 'خطای نامشخص';
    }

    /**
     * Get route path
     * @param {number} routeId - Route ID
     * @returns {string} - Route path
     */
    getRoutePath(routeId) {
        const routes = {
            3: '/dashboard',
            13: '/reset-password'
        };
        
        return routes[routeId] || '/';
    }

    /**
     * Get supplier logo
     * @returns {string} - Supplier logo URL
     */
    getSupplierLogo() {
        return 'https://s.isaco.ir/supplier-logo.png';
    }

    /**
     * Check if user is logged in
     * @returns {boolean} - True if logged in
     */
    isUserLoggedIn() {
        return localStorage.getItem('userToken') !== null;
    }

    /**
     * Validate username
     * @param {Object} options - Validation options
     * @returns {boolean} - True if valid
     */
    isValidUsername(options) {
        const { Value, Title } = options;
        return Value && Value.length >= 2;
    }

    /**
     * Validate date time
     * @param {number} dateTime - Date time
     * @returns {boolean} - True if valid
     */
    isValidDateTime(dateTime) {
        return dateTime && !isNaN(dateTime);
    }

    /**
     * Validate message date time
     * @param {number} dateTime - Date time
     * @returns {boolean} - True if valid
     */
    isValidMessageDateTime(dateTime) {
        return dateTime && !isNaN(dateTime);
    }

    /**
     * Call service
     * @param {Object} options - Service options
     * @returns {Promise} - Service response
     */
    async callService(options) {
        try {
            const response = await fetch('/api/service', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(options)
            });
            
            return await response.json();
        } catch (error) {
            console.error('Service call error:', error);
            throw error;
        }
    }

    /**
     * Save to local storage
     * @param {Object} data - Data to save
     */
    saveToLocalStorage(data) {
        if (data.saveLocal) {
            localStorage.setItem(data.localName, JSON.stringify(data.data));
        }
    }

    /**
     * Get from local storage
     * @param {Object} options - Storage options
     * @returns {*} - Stored data
     */
    getFromLocalStorage(options) {
        if (options.saveLocal) {
            const data = localStorage.getItem(options.localName);
            return data ? JSON.parse(data) : null;
        }
        return options.data;
    }

    /**
     * Show notification
     * @param {string} message - Notification message
     * @param {string} type - Notification type
     */
    showNotification(message, type) {
        const notification = {
            title: 'اطلاعیه',
            timeout: 5000
        };
        
        switch (type) {
            case 'success':
                console.log('Success:', message);
                break;
            case 'warning':
                console.warn('Warning:', message);
                break;
            case 'error':
                console.error('Error:', message);
                break;
            default:
                console.log('Info:', message);
        }
    }
}

// ============================================================================
// CLEAN STYLED COMPONENTS
// ============================================================================

/**
 * Clean Form Component
 */
const Form = styled.form`
    display: flex;
    flex-direction: column;
    gap: 1rem;
    padding: 2rem;
    background: #fff;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
`;

/**
 * Clean Form Title
 */
const FormTitle = styled.h2`
    text-align: center;
    color: #333;
    margin-bottom: 1rem;
`;

/**
 * Clean Form Input
 */
const FormInput = styled.input`
    width: 100%;
    padding: 0.75rem;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 1rem;
    
    &:focus {
        outline: none;
        border-color: #007bff;
    }
`;

/**
 * Clean Submit Button
 */
const SubmitButton = styled.input`
    background: #007bff;
    color: white;
    border: none;
    padding: 0.75rem 1.5rem;
    border-radius: 4px;
    cursor: pointer;
    font-size: 1rem;
    
    &:hover {
        background: #0056b3;
    }
`;

/**
 * Clean SMS Button
 */
const SMSButton = styled.input`
    background: #28a745;
    color: white;
    border: none;
    padding: 0.75rem 1.5rem;
    border-radius: 4px;
    cursor: pointer;
    font-size: 1rem;
    
    &:hover:not(:disabled) {
        background: #1e7e34;
    }
    
    &:disabled {
        background: #6c757d;
        cursor: not-allowed;
    }
`;

/**
 * Clean Reset Button
 */
const ResetButton = styled.input`
    background: #dc3545;
    color: white;
    border: none;
    padding: 0.75rem 1.5rem;
    border-radius: 4px;
    cursor: pointer;
    font-size: 1rem;
    
    &:hover {
        background: #c82333;
    }
`;

/**
 * Clean Captcha Container
 */
const CaptchaContainer = styled.div`
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 1rem;
    background: #f8f9fa;
    border-radius: 4px;
    background-image: url(${props => `https://s.isaco.ir/captcha-bg-${props.bg}.png`});
    background-size: cover;
`;

/**
 * Clean Refresh Button
 */
const RefreshButton = styled.button`
    background: #6c757d;
    color: white;
    border: none;
    padding: 0.5rem;
    border-radius: 4px;
    cursor: pointer;
    
    &:hover {
        background: #545b62;
    }
`;

/**
 * Clean Loading Spinner
 */
const LoadingSpinner = styled.div`
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100px;
    
    &::after {
        content: '';
        width: 40px;
        height: 40px;
        border: 4px solid #f3f3f3;
        border-top: 4px solid #007bff;
        border-radius: 50%;
        animation: spin 1s linear infinite;
    }
    
    @keyframes spin {
        0% { transform: rotate(0deg); }
        100% { transform: rotate(360deg); }
    }
`;

/**
 * Clean Supplier Logo
 */
const SupplierLogo = styled.div`
    display: flex;
    justify-content: center;
    margin-top: 2rem;
    
    img {
        max-width: 200px;
        height: auto;
    }
`;

// ============================================================================
// EXPORTS
// ============================================================================

// Export for Node.js
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        CleanLoginForm,
        CleanGeneratorFactory,
        CleanAsyncGenerator,
        CleanAsyncIterator
    };
}

// Export for browser
if (typeof window !== 'undefined') {
    window.CleanLoginModule = {
        CleanLoginForm,
        CleanGeneratorFactory,
        CleanAsyncGenerator,
        CleanAsyncIterator
    };
}

// Auto-initialize if in browser environment
if (typeof window !== 'undefined' && !window.CleanLoginInitialized) {
    window.CleanLoginInitialized = true;
    console.log('Clean Login Module is ready!');
}
