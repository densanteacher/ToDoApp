TODO: テーブル作成用のスクリプトを用意しておくとよいでしょう。
または、初回実行時にプログラム内で、テーブルがなければ作成してもよいでしょう。

-- Table: public.todo_items

-- DROP TABLE IF EXISTS public.todo_items;

CREATE TABLE IF NOT EXISTS public.todo_items
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( CYCLE INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 999 CACHE 1 ),
    title text COLLATE pg_catalog."default" NOT NULL,
    date_start timestamp without time zone,
    date_end timestamp without time zone NOT NULL,
    memo text COLLATE pg_catalog."default",
    image bytea,
    check_done boolean DEFAULT false,
    priority integer NOT NULL DEFAULT 0,
    remind boolean NOT NULL DEFAULT false,
    remind_date date,
    CONSTRAINT todo_items_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.todo_items
    OWNER to postgres;