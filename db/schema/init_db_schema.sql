-- init roles, grants, db, schema

-- Part I
---------------------------
-- run this under the sa --
---------------------------
-- 1. create roles
create role catebi_admin_role;
create role catebi_support_role;
create role catebi_user_role; -- отдельная роль для приклада
create role catebi_admin with login password 'password';
create role catebi_app with login password 'password';
grant catebi_admin_role to catebi_admin;
grant catebi_user_role to catebi_app;

-- 2. create db
-- run separately
create database catebi with owner catebi_admin_role;

-- 3. create schema
grant connect on database catebi to catebi_admin_role, catebi_user_role, catebi_support_role;

--create extension if not exists "uuid-ossp" schema pg_catalog version "1.1";

-- 4. map search_paths, catebi_admin to catebi_admin_role
alter role catebi_admin set search_path to ctb, information_schema; -- search_path делаем пользователю, а не на всей БД
alter role catebi_admin set role = catebi_admin_role; -- сразу делаем admin_role
alter role catebi_user_role set search_path to ctb, information_schema; -- search_path делаем пользователю, а не на всей БД

-- Part II
-- 5. create new connection
---------------------------------------------------------------------------
-- connect to catebi with catebi_admin, %password% and catebi_admin_role --
---------------------------------------------------------------------------
create schema ctb authorization catebi_admin_role;
grant usage on schema ctb to catebi_admin_role, catebi_user_role, catebi_support_role;

alter default privileges for role catebi_admin_role in schema ctb grant all on tables to catebi_user_role;
alter default privileges for role catebi_admin_role in schema ctb grant select on tables to catebi_support_role;
alter default privileges for role catebi_admin_role in schema ctb grant all on functions to catebi_user_role;
alter default privileges for role catebi_admin_role in schema ctb grant all on sequences to catebi_user_role;
alter default privileges for role catebi_admin_role in schema ctb grant all on types to catebi_user_role;
