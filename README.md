# MesaDeAyuda

![.NET](https://img.shields.io/badge/.NET-10-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)
![License](https://img.shields.io/badge/license-MIT-green)

Sistema web de gestión de tickets e incidencias desarrollado en ASP.NET Core MVC y SQL Server.

## Descripción

MesaDeAyuda es una plataforma diseñada para centralizar la gestión de solicitudes de soporte técnico dentro de una organización.

El sistema permite a los usuarios registrar incidencias, realizar seguimiento a sus solicitudes y consultar el historial de atención. Por su parte, el personal de soporte puede gestionar tickets, asignar responsables, actualizar estados y generar reportes de desempeño.

## Características principales

* Autenticación de usuarios.
* Gestión de roles y permisos.
* Creación de tickets de soporte.
* Asignación de técnicos.
* Seguimiento del ciclo de vida de los tickets.
* Historial de cambios y auditoría.
* Gestión de usuarios.
* Comentarios sobre tickets.
* Adjuntos y evidencias.
* Reportes exportables a Excel.
* Dashboard administrativo.
* Soporte para tema claro y oscuro.

## Roles disponibles

* Administrador
* Soporte de TI
* Usuario
* Gerencia

## Tecnologías utilizadas

### Backend

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server

### Frontend

* Razor Pages
* Bootstrap
* JavaScript
* CSS personalizado

### Seguridad

* Hash de contraseñas
* Control de acceso basado en roles
* Autenticación mediante cookies

## Estructura del proyecto

```text
Controllers/
Data/
Database/
Helpers/
Models/
Views/
wwwroot/
```

## Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/Alezululo/MesaDeAyuda.git
```

### 2. Crear la base de datos

Ejecutar el archivo:

```text
Database/Schema_and_Data.sql
```

en SQL Server Management Studio.

### 3. Configurar la cadena de conexión

Editar el archivo:

```text
appsettings.json
```

y actualizar la cadena de conexión según el entorno local.

### 4. Ejecutar la aplicación

```bash
dotnet restore
dotnet build
dotnet run
```

## Credenciales de prueba

### Administrador

Usuario:

```text
admin
```

Contraseña:

```text
1234
```

Usuario:

```text
azuluaga
```

Contraseña:

```text
1234
```

### Soporte de TI

Usuario:

```text
lgomez
```

Contraseña:

```text
1234
```

Usuario:

```text
mherrera
```

Contraseña:

```text
1234
```

### Usuario

Usuario:

```text
jperez
```

Contraseña:

```text
1234
```

Usuario:

```text
epalacio
```

Contraseña:

```text
1234
```

### Gerencia

Usuario:

```text
cmartinez
```

Contraseña:

```text
1234
```

## Capturas de pantalla

### Inicio de sesión

![Login](Docs/Images/login.png)

### Dashboard principal

![Dashboard](Docs/Images/dashboard.png)

### Gestión de tickets

![Tickets](Docs/Images/gestion-tickets.png)

### Gestión de usuarios

![Usuarios](Docs/Images/gestion-usuarios.png)

### Informes

![Informes](Docs/Images/informes.png)

## Integración Continua (CI)

El repositorio puede utilizar GitHub Actions para ejecutar automáticamente:

* Restauración de dependencias
* Compilación del proyecto
* Validación de errores de compilación

Cada cambio enviado al repositorio puede ser verificado automáticamente antes de su despliegue.

## Autor

Alejandro Zuluaga López

Proyecto desarrollado como evidencia final del programa Tecnólogo en Análisis y Desarrollo de Software del SENA.
