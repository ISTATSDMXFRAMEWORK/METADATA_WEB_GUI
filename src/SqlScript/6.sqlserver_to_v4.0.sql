-- Upgrade script to 4.0 from 3.3

-- increase size of DB_NAME and ADO/JDBC connection strings for SDMXRI-50.

ALTER TABLE DB_CONNECTION ALTER COLUMN DB_NAME VARCHAR(1000) NOT NULL
;
ALTER TABLE DB_CONNECTION  ALTER COLUMN ADO_CONNECTION_STRING VARCHAR(2000)
;
ALTER TABLE DB_CONNECTION  ALTER COLUMN JDBC_CONNECTION_STRING VARCHAR(2000)
;

-- attribute attachment measure table
CREATE TABLE ATT_MEASURE ( 
	MEASURE_COMP_ID bigint NOT NULL,
	ATT_COMP_ID bigint NOT NULL
)
;

ALTER TABLE ATT_MEASURE ADD CONSTRAINT PK_ATT_MEASURE 
	PRIMARY KEY CLUSTERED (MEASURE_COMP_ID, ATT_COMP_ID)
;

ALTER TABLE ATT_MEASURE ADD CONSTRAINT FK_ATT_MEASURE_ATT 
	FOREIGN KEY (ATT_COMP_ID) REFERENCES COMPONENT (COMP_ID)
;

ALTER TABLE ATT_MEASURE ADD CONSTRAINT FK_ATT_MEASURE_DIM 
	FOREIGN KEY (MEASURE_COMP_ID) REFERENCES COMPONENT (COMP_ID)
	ON DELETE CASCADE
;

-- move data from CATEGORISATION.DC_ORDER to ANNOTATION
begin
declare @sysid bigint, @ann_id bigint, @corder bigint, @pk bigint, @corder_txt nvarchar(50);

declare cat_cursor cursor for 
select CATN_ID as SYSID, DC_ORDER as CORDER FROM CATEGORISATION WHERE DC_ORDER IS NOT NULL AND CATN_ID NOT IN (SELECT AA.ART_ID FROM ARTEFACT_ANNOTATION AA INNER JOIN ANNOTATION A ON A.ANN_ID = AA.ANN_ID WHERE A.TYPE='CategoryScheme_node_order') ;

open cat_cursor;

FETCH NEXT from cat_cursor into @sysid, @corder;
while @@FETCH_STATUS <> -1
begin;
	exec INSERT_ARTEFACT_ANNOTATION @p_art_id = @sysid, @p_id = null, @p_title = null, @p_type = 'CategoryScheme_node_order', @p_url = null, @p_pk = @ann_id OUTPUT;
	set @corder_txt = CAST(@corder AS NVARCHAR(50));
	if @corder_txt is null
		set @corder_txt = '0';		
	exec INSERT_ANNOTATION_TEXT @p_ann_id = @ann_id, @p_language = 'en', @p_text = @corder_txt, @p_pk = null;
	FETCH NEXT from cat_cursor into @sysid, @corder;
end;

close cat_cursor;
deallocate cat_cursor;
end
;
go
;

-- move data from CATEGORY_SCHEME.CS_ORDER to ANNOTATION
begin
declare @sysid bigint, @ann_id bigint, @corder bigint, @pk bigint, @corder_txt nvarchar(50);

declare cat_cursor cursor for 
select CAT_SCH_ID as SYSID, CS_ORDER as CORDER FROM CATEGORY_SCHEME WHERE CS_ORDER IS NOT NULL AND CAT_SCH_ID NOT IN (SELECT AA.ART_ID FROM ARTEFACT_ANNOTATION AA INNER JOIN ANNOTATION A ON A.ANN_ID = AA.ANN_ID WHERE A.TYPE='CategoryScheme_node_order') ;

open cat_cursor;

FETCH NEXT from cat_cursor into @sysid, @corder;
while @@FETCH_STATUS <> -1
begin;
	exec INSERT_ARTEFACT_ANNOTATION @p_art_id = @sysid, @p_id = null, @p_title = null, @p_type = 'CategoryScheme_node_order', @p_url = null, @p_pk = @ann_id OUTPUT;
	set @corder_txt = CAST(@corder AS NVARCHAR(50));
	if @corder_txt is null
		set @corder_txt = '0';		
	exec INSERT_ANNOTATION_TEXT @p_ann_id = @ann_id, @p_language = 'en', @p_text = @corder_txt, @p_pk = null;
	FETCH NEXT from cat_cursor into @sysid, @corder;
end;

close cat_cursor;
deallocate cat_cursor;
end
;
go
;

-- move data from CATEGORY.CORDER to ANNOTATION
begin
declare @sysid bigint, @ann_id bigint, @corder bigint, @pk bigint, @corder_txt nvarchar(50);

declare cat_cursor cursor for 
select CAT_ID as SYSID, CORDER FROM CATEGORY WHERE CORDER IS NOT NULL AND CAT_ID NOT IN (SELECT AA.ITEM_ID FROM ITEM_ANNOTATION AA INNER JOIN ANNOTATION A ON A.ANN_ID = AA.ANN_ID WHERE A.TYPE='CategoryScheme_node_order') ;

open cat_cursor;

FETCH NEXT from cat_cursor into @sysid, @corder;
while @@FETCH_STATUS <> -1
begin;
	exec INSERT_ITEM_ANNOTATION @p_item_id = @sysid, @p_id = null, @p_title = null, @p_type = 'CategoryScheme_node_order', @p_url = null, @p_pk = @ann_id OUTPUT;
	set @corder_txt = CAST(@corder AS NVARCHAR(50));
	if @corder_txt is null
		set @corder_txt = '0';		
	exec INSERT_ANNOTATION_TEXT @p_ann_id = @ann_id, @p_language = 'en', @p_text = @corder_txt, @p_pk = null;
	FETCH NEXT from cat_cursor into @sysid, @corder;
end;

close cat_cursor;
deallocate cat_cursor;
end
;
go
;

-- update stored procedures
IF OBJECT_ID('INSERT_CATEGORY') IS NOT NULL
DROP PROC INSERT_CATEGORY
;
GO
;
CREATE PROCEDURE INSERT_CATEGORY
	@p_id varchar(50), 
	@p_cat_sch_id bigint,
	@p_parent_cat_id bigint = null,
	@p_pk bigint OUT 
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	DECLARE @starttrancount int;
	SET @starttrancount = @@TRANCOUNT;
	BEGIN TRY
		IF @starttrancount = 0 
			BEGIN TRANSACTION

		insert into ITEM (ID) VALUES (@p_id);
		set @p_pk = SCOPE_IDENTITY();
		insert into CATEGORY (CAT_ID, CAT_SCH_ID, PARENT_CAT_ID) VALUES (@p_pk, @p_cat_sch_id, @p_parent_cat_id);
	
		IF @starttrancount = 0 
	        COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		IF XACT_STATE() <> 0 AND @starttrancount = 0 
			ROLLBACK TRANSACTION
	   RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH
END
;
GO
;
IF OBJECT_ID('INSERT_CATEGORISATION') IS NOT NULL
DROP PROC INSERT_CATEGORISATION
;
GO
;

CREATE PROCEDURE INSERT_CATEGORISATION
	@p_id varchar(50),
	@p_version varchar(50),
	@p_agency varchar(50),
	@p_valid_from datetime = NULL,
	@p_valid_to datetime= NULL,
	@p_is_final int = NULL,
	@p_uri nvarchar(255),
	@p_last_modified datetime = NULL,
	@p_art_id bigint,
    @p_cat_id bigint,
	@p_pk bigint out
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	DECLARE @starttrancount int;
	SET @starttrancount = @@TRANCOUNT;
	BEGIN TRY
		IF @starttrancount = 0 
			BEGIN TRANSACTION

	exec INSERT_ARTEFACT @p_id = @p_id,@p_version = @p_version,@p_agency = @p_agency,@p_valid_from = @p_valid_from,@p_valid_to = @p_valid_to,@p_is_final = @p_is_final,@p_uri=@p_uri,@p_last_modified=@p_last_modified,@p_pk = @p_pk OUTPUT;
	
INSERT INTO CATEGORISATION
           (CATN_ID
           ,ART_ID
           ,CAT_ID)
     VALUES
           (@p_pk
           ,@p_art_id
           ,@p_cat_id);

	IF @starttrancount = 0 
	        COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		IF XACT_STATE() <> 0 AND @starttrancount = 0 
			ROLLBACK TRANSACTION
	   RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH
END
;
GO
;
IF OBJECT_ID('INSERT_CATEGORY_SCHEME') IS NOT NULL
DROP PROC INSERT_CATEGORY_SCHEME
;
GO
;
CREATE PROCEDURE INSERT_CATEGORY_SCHEME
	@p_id varchar(50),
	@p_version varchar(50),
	@p_agency varchar(50),
	@p_valid_from datetime = NULL,
	@p_valid_to datetime= NULL,
	@p_is_final int = NULL,
	@p_uri nvarchar(255),
	@p_last_modified datetime = NULL,
    @p_is_partial bit = 0,
	@p_pk bigint out
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	DECLARE @starttrancount int;
	SET @starttrancount = @@TRANCOUNT;
	BEGIN TRY
		IF @starttrancount = 0 
			BEGIN TRANSACTION

	exec INSERT_ARTEFACT @p_id = @p_id,@p_version = @p_version,@p_agency = @p_agency,@p_valid_from = @p_valid_from,@p_valid_to = @p_valid_to,@p_is_final = @p_is_final,@p_uri=@p_uri,@p_last_modified = @p_last_modified,@p_pk = @p_pk OUTPUT;
	
	INSERT INTO CATEGORY_SCHEME
           (CAT_SCH_ID, IS_PARTIAL)
     VALUES
           (@p_pk, @p_is_partial);

	IF @starttrancount = 0 
	        COMMIT TRANSACTION

	END TRY
	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		IF XACT_STATE() <> 0 AND @starttrancount = 0 
			ROLLBACK TRANSACTION
	   RAISERROR (@ErrorMessage, -- Message text.
				   @ErrorSeverity, -- Severity.
				   @ErrorState -- State.
				   );
	END CATCH
END
;
GO
;

-- drop ORDER columns

ALTER TABLE CATEGORISATION DROP COLUMN DC_ORDER
;

ALTER TABLE CATEGORY DROP COLUMN CORDER
;

ALTER TABLE CATEGORY_SCHEME  DROP COLUMN CS_ORDER
;

-- always last
update DB_VERSION SET VERSION = '4.0'
;
