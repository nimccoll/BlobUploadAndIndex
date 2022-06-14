using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoadIndex
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Document
    {
        public string ORDER_NUM { get; set; }
        public string PREMISES { get; set; }
        public string ZIP_CODE { get; set; }
        public string CITY { get; set; }
        public string MAPGRID_NUM { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string ASSIGN { get; set; }
        public string PRIORITY { get; set; }
        public string STATUS { get; set; }
        public string STATUS_DATE { get; set; }
        public string CREATE_DATE { get; set; }
        public string START_DATETIME { get; set; }
        public string ISR_NO { get; set; }
        public string ADDRESS { get; set; }
        public string SERVICE_TYPE { get; set; }
        public string APPT { get; set; }
        public string METER_SIZE { get; set; }
        public string MTR_LOCATION { get; set; }
        public string APPT_END_TIME { get; set; }
        public string SERVICEGROUP { get; set; }
        public string DATE_CREATED { get; set; }
        public string DATE_COMPLETED { get; set; }
        public string ENROUTE_DATETIME { get; set; }
        public string AT_LOC_DATETIME { get; set; }
        public string ORDER_DATE { get; set; }
        public string SERVICEMAN { get; set; }
        public string ON_CODE { get; set; }
        public string OFF_CODE { get; set; }
        public string VISUAL_MTR_INSP_GAS { get; set; }
        public string OFF_MTR_RD { get; set; }
        public string METER_RDG { get; set; }
        public string ON_MTR_RD { get; set; }
        public string ORDER_INSTRUCTIONS { get; set; }
        public string COMPLETION_COMMENTS { get; set; }
        public string ORDER_TYPE { get; set; }
        public string METER_NUM { get; set; }
        public string METER_TEST { get; set; }
        public string SERVICE_TEST { get; set; }
        public string HOUSE_LIST_TEST { get; set; }
        public string CC_CODE { get; set; }
        public string DATE_REQUESTED { get; set; }
        public string STATUS_CODE { get; set; }
        public string ACTIVATION_NUM { get; set; }
        public string ST_CUT_PERMIT_NUM { get; set; }
        public string INTER_NUM { get; set; }
        public string STREET_CLASS { get; set; }
        public string MEASUREMENT { get; set; }
        public string TYPE_OF_REPAIR { get; set; }
        public string DEPTH { get; set; }
        public string LENGTH { get; set; }
        public string TABULATED_DATA_FOR_EQUIP_USED { get; set; }
        public string WIDTH { get; set; }
        public string NUM_OF_HYDR_JACKS_USER { get; set; }
        public string CREW_LEAD_FOREMAN { get; set; }
        public string STEEL_PLATE_HALF_IN { get; set; }
        public string STEEL_PLATE_3QTR_INCH { get; set; }
        public string STEEL_PLATE_OTHER { get; set; }
        public string FINFORM_3QTR_INCH { get; set; }
        public string SHORTING_BOX { get; set; }
        public string SLOPE_RATIO { get; set; }
        public string LOCATE_REFERENCE_NUM { get; set; }
        public string MEASUREMENTS { get; set; }
        public string WAS_UTILITY_MARKED { get; set; }
        public string DISTANCE_UTILITY_MARKED { get; set; }
        public string EQUIPMENT_OR_TOOL_CAUSING_DAMAGE { get; set; }
        public string OPERATOR_OR_EMPLOYEE { get; set; }
        public string UNDERGROUND_MAINTENANCE_FITTER { get; set; }
        public string UTILITY_REP_NAME { get; set; }
        public string SIZE_OF_LINE { get; set; }
        public string LINE { get; set; }
        public string NUM_OF_CUSTOMERS_AFFECTED { get; set; }
        public string LINE_BLOWING { get; set; }
        public string FIRE_DEPT_RESPOND { get; set; }
        public string POLICE_DEPT_RESPOND { get; set; }
        public string CUSTOMERS_EVACUATED { get; set; }
        public string NUM_OF_CUSTOMERS_EVACUATED { get; set; }
        public string IGNITION_OR_FIRE { get; set; }
        public string NOTIFIED_811_OF_HIT_LINE { get; set; }
        public string INJURIES_FATALITIES { get; set; }
        public string PICTURES_TAKEN { get; set; }
        public string NAMES { get; set; }
        public string FINISH_COMMENTS { get; set; }
        public string COMPLETED_COMMENTS { get; set; }
        public string FINISH_COMMENTS_NEW1 { get; set; }
        public string ADDITIONAL_COMMENTS { get; set; }
        public string CS_NEED_DATE { get; set; }
        public string OFF_MTR_NUM { get; set; }
        public string DISPATCH_DATETIME { get; set; }
        public string CREATE_ORDER { get; set; }
        public string DISPATCH_BY { get; set; }
        public string ERT_SERIAL_NUM { get; set; }
        public string ERT_TYPE { get; set; }
        public string ERT_MANUFACTURE { get; set; }
        public string RMV_REASON { get; set; }
        public string NEW_METER_NUM { get; set; }
        public string START_AFTER_DATETIME { get; set; }
        public string SERVICE_PT_TYPE { get; set; }
        public string SVC_STATUS { get; set; }
        public string P_FILTER_1 { get; set; }
        public string Test_Group { get; set; }
        public string Orig_install_date { get; set; }
        public string DIST_MAP { get; set; }
        public string SVC_PT_WARNING { get; set; }
        public string CURR_DISCONNECT_LOC { get; set; }
        public string GAS_PRESSURE { get; set; }
        public string DISP_LEAK_INFORMATION { get; set; }
        public double AT_LOC_MIN { get; set; }
        public string SUPERVISOR_NAME { get; set; }
        public string ORDER_TYPE_DESCRIPTION { get; set; }
        public string BOX_MEASUREMENT { get; set; }
        public double LATTITUDE { get; set; }
        public double LONGITUDE { get; set; }
        public string PCAD_MTR_STATUS { get; set; }
        public string MTR_MANF_CODE { get; set; }
        public string SVC_PT_INSTR_DETAILS { get; set; }
        public string MTR_CHG_FLG { get; set; }
        public string cmPcadJobNumber { get; set; }
        public string OPERATING_COMPANY { get; set; }
        public string date_created_sld { get; set; }
        public string date_completed_sld { get; set; }
        public string enroute_datetime_sld { get; set; }
        public string at_loc_time_sld { get; set; }
        public string order_date_sld { get; set; }
        public string date_requested_sld { get; set; }
        public string cs_need_date_sld { get; set; }
        public string NEW_ERT_SERIAL_NUM { get; set; }
        public string ISR_MOD_DATETIME { get; set; }
        public string UpdateDateTimeUtc { get; set; }
    }

    public class Documents
    {
        public List<Document> value { get; set; }
    }
}
