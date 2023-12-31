USE [master]
GO
/****** Object:  Database [BdiExamen]    Script Date: 29/10/2023 05:30:21 p. m. ******/
CREATE DATABASE [BdiExamen]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BdiExamen', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\BdiExamen.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BdiExamen_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\BdiExamen_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BdiExamen].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BdiExamen] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BdiExamen] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BdiExamen] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BdiExamen] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BdiExamen] SET ARITHABORT OFF 
GO
ALTER DATABASE [BdiExamen] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BdiExamen] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BdiExamen] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BdiExamen] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BdiExamen] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BdiExamen] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BdiExamen] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BdiExamen] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BdiExamen] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BdiExamen] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BdiExamen] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BdiExamen] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BdiExamen] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BdiExamen] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BdiExamen] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BdiExamen] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BdiExamen] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BdiExamen] SET RECOVERY FULL 
GO
ALTER DATABASE [BdiExamen] SET  MULTI_USER 
GO
ALTER DATABASE [BdiExamen] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BdiExamen] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BdiExamen] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BdiExamen] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
USE [BdiExamen]
GO
/****** Object:  Table [dbo].[tblExamen]    Script Date: 29/10/2023 05:30:21 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblExamen](
	[idExamen] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](255) NULL,
	[Descripcion] [varchar](255) NULL,
 CONSTRAINT [PK_tblExamen] PRIMARY KEY CLUSTERED 
(
	[idExamen] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spActualizar]    Script Date: 29/10/2023 05:30:21 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spActualizar]
	-- Parámetros
	@Id int,
	@Nombre varchar(255),
	@Descripcion varchar(255)
AS
BEGIN
--Mensaje de error
	declare @mensaje varchar(500) = 'ok'
--Código de estatus
	declare @estatus int = 0

    begin try
	--Validar que exista un registro con el id proporcionado
	if not exists(select idExamen from tblExamen where idExamen = @Id)
	begin;
		throw 50003, 'No existe un examen con el ID proporcionado',1;
	end;
	--Validar que el nombre y la descripción no sean un espacio en blanco o null
	 if trim(@Nombre) is null or len(@Nombre) = 0
	 begin;
		throw 50001, 'El nombre no puede estar vacío',1;
	 end;

	 if trim(@Descripcion) is null or len(@Descripcion) = 0
	 begin;
		throw 50002, 'La descripción no puede estar vacía',1;
	 end;
	 --Fin validaciones

	 --Actualizar información
	 update tblExamen set Nombre = @Nombre, Descripcion = @Descripcion where idExamen = @Id

	end try
	begin catch
	--Cachar el error
	set @mensaje = cast(ERROR_MESSAGE() as varchar(500))
	set @estatus = ERROR_NUMBER()
	end catch
	--Devolver el código de estado y el mensaje de resultado
	select @estatus as Estatus, @mensaje as Mensaje
END
GO
/****** Object:  StoredProcedure [dbo].[spAgregar]    Script Date: 29/10/2023 05:30:21 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spAgregar]
	-- Parámetros
	@Nombre varchar(255),
	@Descripcion varchar(255)
AS
BEGIN
--Mensaje de error
	declare @mensaje varchar(500) = 'ok'
--Código de estatus
	declare @estatus int = 0

    begin try
	--Validar que el nombre y la descripción no sean un espacio en blanco o null
	 if trim(@Nombre) is null or len(@Nombre) = 0
	 begin;
		throw 50001, 'El nombre no puede estar vacío',1;
	 end;

	 if trim(@Descripcion) is null or len(@Descripcion) = 0
	 begin;
		throw 50002, 'La descripción no puede estar vacía',1;
	 end;
	 --Fin validaciones

	 --Insertar información
	 insert into tblExamen(Nombre,Descripcion) values(@Nombre,@Descripcion)

	end try
	begin catch
	--Cachar el error
	set @mensaje = cast(ERROR_MESSAGE() as varchar(500))
	set @estatus = ERROR_NUMBER()
	end catch
	--Devolver el código de estado y el mensaje de resultado
	select @estatus as Estatus, @mensaje as Mensaje
END
GO
/****** Object:  StoredProcedure [dbo].[spConsultar]    Script Date: 29/10/2023 05:30:21 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spConsultar]
	-- Parámetros
	@id int,
	@Nombre varchar(255),
	@Descripcion varchar(255)
AS
BEGIN
--Buscar coincidencias
if(@id > 0 or len(@Nombre) > 0 or len(@Descripcion) > 0)
begin
--Filtrar
select * from tblExamen 
where (@id > 0 and idExamen like concat('%', @id, '%'))
or (LEN(@Nombre) > 0 and Nombre like '%' + @Nombre + '%')
or (LEN(@Descripcion) > 0 and Descripcion like '%' + @Descripcion + '%')
end
else
begin
--Traer todo
select * from tblExamen
end

END
GO
/****** Object:  StoredProcedure [dbo].[spEliminar]    Script Date: 29/10/2023 05:30:21 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spEliminar]
	-- Parámetros
	@Id int
AS
BEGIN
--Mensaje de error
	declare @mensaje varchar(500) = 'ok'
--Código de estatus
	declare @estatus int = 0

    begin try
	--Validar que exista un registro con el id proporcionado
	if not exists(select idExamen from tblExamen where idExamen = @Id)
	begin;
		throw 50003, 'No existe un examen con el ID proporcionado',1;
	end;
	 --Eliminar Registro
	 delete from tblExamen where idExamen = @Id
	end try
	begin catch
	--Cachar el error
	set @mensaje = cast(ERROR_MESSAGE() as varchar(500))
	set @estatus = ERROR_NUMBER()
	end catch
	--Devolver el código de estado y el mensaje de resultado
	select @estatus as Estatus, @mensaje as Mensaje
END
GO
USE [master]
GO
ALTER DATABASE [BdiExamen] SET  READ_WRITE 
GO
