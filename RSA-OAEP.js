window.crypto.subtle.generateKey(
    {
        name: "RSA-OAEP",
        modulusLength: 2048, //can be 1024, 2048, or 4096
        publicExponent: new Uint8Array([0x01, 0x00, 0x01]),
        hash: {name: "SHA-256"}, //can be "SHA-1", "SHA-256", "SHA-384", or "SHA-512"
    },
    false, //whether the key is extractable (i.e. can be used in exportKey)
    ["encrypt", "decrypt"] //must be ["encrypt", "decrypt"] or ["wrapKey", "unwrapKey"]
)
.then(function(key){
    //returns a keypair object
    console.log(key);
    console.log(key.publicKey);
    console.log(key.privateKey);
})
.catch(function(err){
    console.error(err);
});

window.crypto.subtle.importKey(
    "jwk", //can be "jwk" (public or private), "spki" (public only), or "pkcs8" (private only)
    {   //this is an example jwk key, other key types are Uint8Array objects
        kty: "RSA",
        e: "AQAB",
        n: "vGO3eU16ag9zRkJ4AK8ZUZrjbtp5xWK0LyFMNT8933evJoHeczexMUzSiXaLrEFSyQZortk81zJH3y41MBO_UFDO_X0crAquNrkjZDrf9Scc5-MdxlWU2Jl7Gc4Z18AC9aNibWVmXhgvHYkEoFdLCFG-2Sq-qIyW4KFkjan05IE",
        alg: "RSA-OAEP-256",
        ext: true,
    },
    {   //these are the algorithm options
        name: "RSA-OAEP",
        hash: {name: "SHA-256"}, //can be "SHA-1", "SHA-256", "SHA-384", or "SHA-512"
    },
    false, //whether the key is extractable (i.e. can be used in exportKey)
    ["encrypt"] //"encrypt" or "wrapKey" for public key import or
                //"decrypt" or "unwrapKey" for private key imports
)
.then(function(publicKey){
    //returns a publicKey (or privateKey if you are importing a private key)
    console.log(publicKey);
})
.catch(function(err){
    console.error(err);
});


window.crypto.subtle.exportKey(
    "jwk", //can be "jwk" (public or private), "spki" (public only), or "pkcs8" (private only)
    publicKey //can be a publicKey or privateKey, as long as extractable was true
)
.then(function(keydata){
    //returns the exported key data
    console.log(keydata);
})
.catch(function(err){
    console.error(err);
});

window.crypto.subtle.encrypt(
    {
        name: "RSA-OAEP",
        //label: Uint8Array([...]) //optional
    },
    publicKey, //from generateKey or importKey above
    data //ArrayBuffer of data you want to encrypt
)
.then(function(encrypted){
    //returns an ArrayBuffer containing the encrypted data
    console.log(new Uint8Array(encrypted));
})
.catch(function(err){
    console.error(err);
});

window.crypto.subtle.decrypt(
    {
        name: "RSA-OAEP",
        //label: Uint8Array([...]) //optional
    },
    privateKey, //from generateKey or importKey above
    data //ArrayBuffer of the data
)
.then(function(decrypted){
    //returns an ArrayBuffer containing the decrypted data
    console.log(new Uint8Array(decrypted));
})
.catch(function(err){
    console.error(err);
});

window.crypto.subtle.wrapKey(
    "raw", //the export format, must be "raw" (only available sometimes)
    key, //the key you want to wrap, must be able to fit in RSA-OAEP padding
    publicKey, //the public key with "wrapKey" usage flag
    {   //these are the wrapping key's algorithm options
        name: "RSA-OAEP",
        hash: {name: "SHA-256"},
    }
)
.then(function(wrapped){
    //returns an ArrayBuffer containing the encrypted data
    console.log(new Uint8Array(wrapped));
})
.catch(function(err){
    console.error(err);
});

window.crypto.subtle.unwrapKey(
    "raw", //the import format, must be "raw" (only available sometimes)
    wrapped, //the key you want to unwrap
    privateKey, //the private key with "unwrapKey" usage flag
    {   //these are the wrapping key's algorithm options
        name: "RSA-OAEP",
        modulusLength: 2048,
        publicExponent: new Uint8Array([0x01, 0x00, 0x01]),
        hash: {name: "SHA-256"},
    },
    {   //this what you want the wrapped key to become (same as when wrapping)
        name: "AES-GCM",
        length: 256
    },
    false, //whether the key is extractable (i.e. can be used in exportKey)
    ["encrypt", "decrypt"] //the usages you want the unwrapped key to have
)
.then(function(key){
    //returns a key object
    console.log(key);
})
.catch(function(err){
    console.error(err);
});