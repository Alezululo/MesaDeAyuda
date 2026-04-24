##### **GUÍA DEL LOGIN**



**Los usuarios de ingreso iniciales son:**



Tipo de cuenta: Usuario

Usuario: jperez

Contraseña: 1234



Tipo de cuenta: Usuario

Usuario: Epalacio

Contraseña: 1234



Tipo de cuenta: Soporte de TI

Usuario: lgomez

Contraseña: 1234



Tipo de cuenta: Soporte de TI

Usuario: mherrera

Contraseña: 1234



Tipo de cuenta: Administrador

Usuario: admin

Contraseña: 1234



Tipo de cuenta: Administrador

Usuario: azuluaga

Contraseña: 1234



Tipo de cuenta: Gerencia

Usuario: cmartinez

Contraseña: 1234







##### **GUÍA DEL CSS**

###### 

###### **site.css:**



Es la base global del sistema.



Aquí van:



* colores y variables (:root)
* modo claro/oscuro
* estilos globales de body
* switch del tema
* badges y colores reutilizables
* estados y prioridades
* filtros genéricos



Si quieres cambiar:



* colores del sistema
* color de un estado
* color de una prioridad
* color de un badge
* apariencia global de selects
* espaciado base de filtros



Se hace aquí.





###### **layout.css:**



Es la estructura general de las páginas.



Aquí van:



* .page-layout
* .sidebar
* .main
* headers estructurales
* distribución general de páginas con menú lateral



Si quieres cambiar:



* ancho del sidebar
* padding del contenido principal
* separación entre menú y contenido
* estilo base del menú lateral
* comportamiento del layout general



lo haces aquí.





###### **components.css:**



Aquí van los bloques reutilizables de interfaz.



Piensa en “piezas” del sistema:



* tablas
* modales
* botones
* paginación
* formularios
* cards pequeñas
* timeline
* inputs reutilizables
* contenedores como .table-container, .modal-content, .form-card



Si quieres cambiar:



* una tabla
* un botón
* un modal
* un input
* un label dentro de formularios/modales
* paginación
* card pequeña de dashboard



lo haces aquí.





###### **pages.css:**



Aquí van estilos específicos de páginas concretas.



O sea, cosas que no son globales ni reutilizables, sino propias de:



* Home
* Login
* Access Denied
* header del home
* cards del home
* perfil visual del home/login



Si quieres cambiar:



* cards del panel principal
* logo del home
* caja del perfil arriba
* formulario de login
* alerta del login
* pantalla de acceso denegado



lo haces aquí.





**Resumen:**

---

* Si afecta a todo el sistema: **site.css**
* Si afecta a la estructura de la página: **layout.css**
* Si afecta a una pieza reutilizable: **components.css**
* Si afecta a una pantalla específica: **pages.css**







##### **GUÍA de las vistas**



###### **Account:**

1. **AccessDenied.cshtml:** Se muestra el mensaje de acceso denegado a una URL no autorizada para el usuario logueado.
2. **Login.cshtml:** Formulario de inicio de sesión.

###### 

###### **Admin:**

1. **AuditoriaTickets.cshtml:** Tabla de registros de cambios en las prioridades y asignaciones de los tickets.
2. **AuditoriaUsuarios.cshtml:** abla de registros de cambios de cualquier indole en las cuentas de usuario.

###### 

###### **Home:**

1. **Index.cshtml:** Menu principal dónde se encuentran todos los modulos.
2. **Privacy.cshtml:** Politica de privacidad de la pagina, en caso de necesitarse para tratamiento de datos.

###### 

###### **Profile:**

1. **Index.cshtml:** Modulo dónde se visualiza el perfil del usuario logueado y sus datos basicos, asi como el cierre de sesión.

###### 

###### **Reports:**

**Index.cshtml:** Modulo dónde se visualizan las metricas de los tecnicos y se exportan a excel.



###### **Tickets:**

1. **CrearTicket.cshtml:** Modulo dónde se llena el formulario de creación de ticket.
2. **GestionTicket.cshtml:** Modulo dónde el personal de TI le da manejo al ticket.
3. **VerTicketsUsuario.cshtml:** Modulo dónde el usuario puede ver los tickets que ha solicitado.
4. **\_DetalleAuditoriaAsignacion.cshtml:** Modal que muestra a detalle el cambio especifico de tecnicos que se hizo.
5. **\_DetalleAuditoriaEstado.cshtml:** Modal que muestra a detalle el cambio especifico de prioridad que se hizo.
6. **\_DetallesTicketAdmin.cshtml:** Modal que muestra a detalle un ticket al personal de soporte de ti.
7. **\_DetallesTicketUsuario.cshtml:** Modal que muestra a detalle como va el proceso de un ticket al usuario.
8. **\_DetalleAuditoriaUsuario.cshtml:** Modal que muestra a detalle los cambios realizados en el usuario.



###### **UserManagement:**

1. **Index.cshtml:** Modulo dónde se ven y gestionan los usuarios del sistema.
2. **\_CrearUsuario.cshtml:** Modal que permite crear un nuevo usuario.
3. **\_EditarUsuario.cshtml:** Modal que permite editar a un usuario existente.



