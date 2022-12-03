# KeePass Certificate Shortcut Provider

A KeePass Provider allowing you to open your database using either a master password OR an X509 certificate.

## Why?

My master password is pretty long.

I want to be able to open my KeePass database using a certificate on a SmartCard as a shortcut, but I still want to be able to use my database on Android, where this plugin would not work.

Unlike the default KeePass behavior when creating a composite key, I don't want the two methods to be additive.

## How?

This provider can generate a .cspkey file (Certificate Shortcut Provider Key) containing the master password encrypted with the public part of an X509 certificate.

When the provider is used, it decrypts the master password using the private part of the certificate, and returns it to KeePass.

This way, it's possible to easily open the database using only a SmartCard.

If required - on a KeePass version without plugins like Android - the database can always be opened using only the master password.

## Is it secure?

It should be.

If you use a certificate with a strong enough key (RSA with at least a 1024 bits key is recommended), the limiting factor will most likely be the strength of your master password.

If you think otherwise, please open an issue...

## Compatibility

This plugin has only been tested on Windows and is unlikely to work on other platforms in its current state, as access to hardware tokens is made through the Windows Certificates Store abstraction.

With some rework, PKCS#11 could probably be used instead to provide multi-platform compatibility, but this remains to be done. [See this issue](https://github.com/mlaily/KeePass-CertificateShortcutProvider/issues/1)

## What kind of certificates can I use?

For now, only RSA certificates are supported.

ECDSA is a signing algorithm. Supporting ECDSA certificates would require some kind of hack to be able to encrypt the master password.
See [here if you feel adventurous](https://stackoverflow.com/questions/47116611/how-can-i-encrypt-data-using-a-public-key-from-ecc-x509-certificate-in-net-fram)...
