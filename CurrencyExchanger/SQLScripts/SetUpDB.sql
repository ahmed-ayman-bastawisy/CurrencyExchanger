﻿USE [master]
GO

/****** Object:  Database [ExchangeRate]    Script Date: 21/11/2022 08:56:48 PM ******/
CREATE DATABASE [ExchangeRate]
 
GO

CREATE TABLE [ExchangeRate].[dbo].[exchanges](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ClientId] [bigint] NOT NULL,
	[PerformedAt] [datetime2](7) NOT NULL,
	[Rate] [decimal](20, 7) NOT NULL,
	[FromAmount] [decimal](20, 7) NOT NULL,
	[ToAmount] [decimal](20, 7) NOT NULL,
	[From] [nvarchar](3) NOT NULL,
	[To] [nvarchar](3) NOT NULL,
	[Succeded] [bit] NOT NULL,
 CONSTRAINT [PK_exchanges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]