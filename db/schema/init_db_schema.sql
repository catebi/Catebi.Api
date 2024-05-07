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

create schema frgn authorization catebi_admin_role;
comment on schema frgn is 'Схема, включающая объекты, связанные с ботом-барахольщиком "Freegan Bot"';
grant usage on schema frgn to catebi_admin_role, catebi_user_role, catebi_support_role;

alter default privileges for role catebi_admin_role in schema frgn grant all on tables to catebi_user_role;
alter default privileges for role catebi_admin_role in schema frgn grant select on tables to catebi_support_role;
alter default privileges for role catebi_admin_role in schema frgn grant all on functions to catebi_user_role;
alter default privileges for role catebi_admin_role in schema frgn grant all on sequences to catebi_user_role;
alter default privileges for role catebi_admin_role in schema frgn grant all on types to catebi_user_role;

create schema schdl authorization catebi_admin_role;
comment on schema schdl is 'Схема, включающая объекты, связанные c ботом-дежурником';
grant usage on schema schdl to catebi_admin_role, catebi_user_role, catebi_support_role;

alter default privileges for role catebi_admin_role in schema schdl grant all on tables to catebi_user_role;
alter default privileges for role catebi_admin_role in schema schdl grant select on tables to catebi_support_role;
alter default privileges for role catebi_admin_role in schema schdl grant all on functions to catebi_user_role;
alter default privileges for role catebi_admin_role in schema schdl grant all on sequences to catebi_user_role;
alter default privileges for role catebi_admin_role in schema schdl grant all on types to catebi_user_role;

create schema tasks authorization catebi_admin_role;
comment on schema tasks is 'Схема, включающая объекты, связанные c tasks ботом';
grant usage on schema tasks to catebi_admin_role, catebi_user_role, catebi_support_role;

alter default privileges for role catebi_admin_role in schema tasks grant all on tables to catebi_user_role;
alter default privileges for role catebi_admin_role in schema tasks grant select on tables to catebi_support_role;
alter default privileges for role catebi_admin_role in schema tasks grant all on functions to catebi_user_role;
alter default privileges for role catebi_admin_role in schema tasks grant all on sequences to catebi_user_role;
alter default privileges for role catebi_admin_role in schema tasks grant all on types to catebi_user_role;