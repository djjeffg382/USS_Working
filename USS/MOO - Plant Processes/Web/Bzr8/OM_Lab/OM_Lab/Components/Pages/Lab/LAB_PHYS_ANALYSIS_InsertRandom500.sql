DECLARE
    v_start_wgt NUMBER(10,2);
    v_inch_1_wgt NUMBER(10,2);
    v_inch_3_4_wgt NUMBER(10,2);
    v_inch_5_8_wgt NUMBER(10,2);
    v_inch_9_16_wgt NUMBER(10,2);
    v_inch_1_2_wgt NUMBER(10,2);
    v_inch_7_16_wgt NUMBER(10,2);
    v_inch_3_8_wgt NUMBER(10,2);
    v_inch_1_4_wgt NUMBER(10,2);
    v_mesh_3_wgt NUMBER(10,2);
    v_mesh_4_wgt NUMBER(10,2);
    v_mesh_6_wgt NUMBER(10,2);
    v_mesh_8_wgt NUMBER(10,2);
    v_mesh_10_12_wgt NUMBER(10,2);
    v_mesh_14_16_wgt NUMBER(10,2);
    v_mesh_20_wgt NUMBER(10,2);
    v_mesh_28_30_wgt NUMBER(10,2);
    v_mesh_35_40_wgt NUMBER(10,2);
    v_mesh_48_50_wgt NUMBER(10,2);
    v_mesh_65_70_wgt NUMBER(10,2);
    v_mesh_100_wgt NUMBER(10,2);
    v_mesh_150_140_wgt NUMBER(10,2);
    v_mesh_200_wgt NUMBER(10,2);
    v_mesh_270_wgt NUMBER(10,2);
    v_mesh_325_wgt NUMBER(10,2);
    v_mesh_400_wgt NUMBER(10,2);
    v_mesh_500_wgt NUMBER(10,2);
BEGIN
    FOR i IN 1..500 LOOP
        v_start_wgt := ROUND(DBMS_RANDOM.VALUE(1000, 10000), 2);
        v_inch_1_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_inch_3_4_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_inch_5_8_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_inch_9_16_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_inch_1_2_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_inch_7_16_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_inch_3_8_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_inch_1_4_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_3_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_4_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_6_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_8_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_10_12_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_14_16_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_20_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_28_30_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_35_40_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_48_50_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_65_70_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_100_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_150_140_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_200_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_270_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_325_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_400_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        v_mesh_500_wgt := ROUND(DBMS_RANDOM.VALUE(0, v_start_wgt), 2);
        INSERT INTO TOLIVE.LAB_PHYS_ANALYSIS (
            LAB_PHYS_ANALYSIS_ID, LAB_PHYS_TYPE_ID, LINE_NBR, ANALYSIS_DATE, SHIFT_DATE8, SHIFT_NBR8, SHIFT_DATE12, SHIFT_NBR12, UPDATE_DATE, LAST_UPDATE_BY, START_WGT,
            INCH_1_WGT, INCH_3_4_WGT, INCH_5_8_WGT, INCH_9_16_WGT, INCH_1_2_WGT, INCH_7_16_WGT, INCH_3_8_WGT, INCH_1_4_WGT,
            MESH_3_WGT, MESH_4_WGT, MESH_6_WGT, MESH_8_WGT, MESH_10_12_WGT, MESH_14_16_WGT, MESH_20_WGT, MESH_28_30_WGT, MESH_35_40_WGT, MESH_48_50_WGT, MESH_65_70_WGT,
            MESH_100_WGT, MESH_150_140_WGT, MESH_200_WGT, MESH_270_WGT, MESH_325_WGT, MESH_400_WGT, MESH_500_WGT,
            SHIFT_HALF8, DEFAULTS_USED, SHIFT_HALF12, AUTHORIZED_BY
        ) VALUES (
            100000+i, -- LAB_PHYS_ANALYSIS_ID
            TRUNC(DBMS_RANDOM.VALUE(1, 100)), -- LAB_PHYS_TYPE_ID
            TRUNC(DBMS_RANDOM.VALUE(1, 20)), -- LINE_NBR
            TRUNC(SYSDATE - DBMS_RANDOM.VALUE(0, 365)), -- ANALYSIS_DATE
            TRUNC(SYSDATE - DBMS_RANDOM.VALUE(0, 365)), -- SHIFT_DATE8
            TRUNC(DBMS_RANDOM.VALUE(0, 9)), -- SHIFT_NBR8
            TRUNC(SYSDATE - DBMS_RANDOM.VALUE(0, 365)), -- SHIFT_DATE12
            TRUNC(DBMS_RANDOM.VALUE(0, 9)), -- SHIFT_NBR12
            SYSDATE, -- UPDATE_DATE
            'USER_' || TRUNC(DBMS_RANDOM.VALUE(1, 100)), -- LAST_UPDATE_BY
            v_start_wgt,
            v_inch_1_wgt, v_inch_3_4_wgt, v_inch_5_8_wgt, v_inch_9_16_wgt, v_inch_1_2_wgt, v_inch_7_16_wgt, v_inch_3_8_wgt, v_inch_1_4_wgt,
            v_mesh_3_wgt, v_mesh_4_wgt, v_mesh_6_wgt, v_mesh_8_wgt, v_mesh_10_12_wgt, v_mesh_14_16_wgt, v_mesh_20_wgt, v_mesh_28_30_wgt, v_mesh_35_40_wgt, v_mesh_48_50_wgt, v_mesh_65_70_wgt,
            v_mesh_100_wgt, v_mesh_150_140_wgt, v_mesh_200_wgt, v_mesh_270_wgt, v_mesh_325_wgt, v_mesh_400_wgt, v_mesh_500_wgt,
            TRUNC(DBMS_RANDOM.VALUE(0, 9)), -- SHIFT_HALF8
            TRUNC(DBMS_RANDOM.VALUE(0, 9)), -- DEFAULTS_USED
            TRUNC(DBMS_RANDOM.VALUE(0, 9)), -- SHIFT_HALF12
            'AUTH_' || TRUNC(DBMS_RANDOM.VALUE(1, 100)) -- AUTHORIZED_BY
        );
    END LOOP;
    COMMIT;
END;
/