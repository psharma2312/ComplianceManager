using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplianceManager
{
    public class MasterData{}
    public class UserNameLst
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class UserRights
    {
        public int ObjectId { get; set; }
        public string ObjectName { get; set; }
        public bool Rights { get; set; }
    }
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }  //loginId
        public string Email { get; set; }
        public string Password { get; set; }
        public string HashCode { get; set; }
        public int SupervisorId { get; set; }
        public int DepartmentId { get; set; }
        public string SupervisorName { get; set; }
        public string DepartmentName { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsApprover { get; set; }
        public bool IsPreparer { get; set; }
        public bool IsSupervisor { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public class Supervisor
    {
        public int SupervisorId { get; set; }
        public string SupervisorName { get; set; }
        public List<User> Team { get; set; }
        public bool IsActive { get; set; }
    }

    public class Department
    {
        public int DeptID { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
    public class LawType
    {
        public int LawID { get; set; }
        public string LawName { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
    public class DriveBy
    {
        public int DrivenId { get; set; }
        public string DrivenName { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }
    public class ComplianceNature
    {
        public int ComplianceNatureID { get; set; }
        public string NatureOfCompliance { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
    }
    public class BusinessUnit
    {
        public int BusinessUnitID { get; set; }
        public string BusinessUnitName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
    }
    public class ComplianceAreas
    {
        public int ComplianceAreaID { get; set; }
        public string ComplianceAreaName { get; set; }
    }
    public class GovLeg
    {
        public string GovLegName { get; set; }
    }
    public class ActSection
    {
        public string ActSectionName { get; set; }
    }
    public class Performance
    {
        public string DepartmentName { get; set; }
        public int NoOfCompliance { get; set; }
        public int ClosedWithinTimeline { get; set; }
        public int ClosedWithDelays { get; set; }
        public Double PercentageOfDelays { get; set; }
        public int PendingSurpassedDueDate { get; set; }
    }
    public class ComplianceSummary
    {
        public int TotalCompliances { get; set; }
        public int PendingCompliances { get; set; }
        public int OverdueCompliances { get; set; }
        public int SentForApproval { get; set; }
    }
    public class ComplianceDelayedResult
    {
        public List<ComplianceMasterr> OverdueCompliances { get; set; }
        public ComplianceSummary Summary { get; set; }
    }
    public class ComplianceMasterr
    {
        public int ComplianceID { get; set; }
        public string ComplianceRef { get; set; }
        public int BusinessUnitID { get; set; }
        public string BusinessUnitName { get; set; }
        public int ComplianceTypeID { get; set; }
        public string ComplianceTypeName { get; set; }
        public int DrivenById { get; set; }
        public string DrivenByName { get; set; }
        //public string ComplianceArea { get; set; }
        public string ActSectionReference { get; set; }
        //public string ComplianceRef { get; set; }
        public string ClauseRef { get; set; }
        public int ComplianceNatureID { get; set; }
        public string NatureOfComplianceName { get; set; }
        public int LawId { get; set; }
        public string LawName { get; set; }
        public int TerritoryId { get; set; }
        public string Territory { get; set; }
        public int PriorityId { get; set; }
        public string Priority { get; set; }
        public int FrequencyID { get; set; }
        public string FrequencyName { get; set; }
        public int OccursEveryID { get; set; }
        public string OccursEveryName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime StandardDueDate { get; set; }
        public int DeptId { get; set; }
        public string DepartmentName { get; set; }
        public int InitiatorId { get; set; }
        public string InitiatorName { get; set; }
        public int ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string FormsIfAny { get; set; }
        public int DueOnEvery { get; set; }
        public int SpecificGeneralId { get; set; }
        public string SpecificGeneral { get; set; }
        public string DetailsOfComplianceRequirements { get; set; }
        public string NonCompliancePenalty { get; set; }
        public string ActionsToBeTaken { get; set; }
        public string TypeOfNonCompliance { get; set; }

        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }
        public string GovernmentLegislation { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public int  AssignedToID { get; set; }
        public string  AssignedToName { get; set; }
        public int ComplianceStatusId { get; set; }
        public string ComplianceStatusName { get; set; }
        public string Remarks { get; set; }
        public DateTime FirstDueDate { get; set; }
        public DateTime ComplianceDueDate { get; set; }
        public string PreparerRemarks { get; set; }
        public string ApproverRemarks { get; set; }
    }
    public class OccursEvery
    {
        public int ID { get; set; }
        public string OccursEveryName { get; set; }
        public bool IsActive { get; set; }
    }
    public class Territory
    {
        public int TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public bool IsActive { get; set; }
    }
    public class Priority
    {
        public int PriorityId { get; set; }
        public string PriorityName { get; set; }
        public bool IsActive { get; set; }
    }
    // Data models for dashboard
    public class DashboardData
    {
        public int CompletedCount { get; set; }
        public int InProgressCount { get; set; }
        public int PendingCount { get; set; }
        public int ClosedOnTimeCount { get; set; }
        public int ClosedWithDelayCount { get; set; }
        public int Due30DaysCount { get; set; }
        public int Due60DaysCount { get; set; }
        public int DueAfter60DaysCount { get; set; }
        public List<CalendarEvent> DueDates { get; set; }
        public List<NonComplianceIssue> NonComplianceIssues { get; set; }
    }
    //public class CalendarEvent
    //{
    //    public string Title { get; set; }
    //    public string Date { get; set; }
    //    public string Description { get; set; }
    //}
    public class MastersName
    {
        public int MasterID { get; set; }
        public string MasterName { get; set; }
    }
    public class NonComplianceIssue
    {
        public string ComplianceType { get; set; }
        public string NonComplianceReasons { get; set; }
    }
    public class ComplianceCertificate
    {
        public int CertificateId { get; set; }
        public string CertificateNo { get; set; }
        public int ComplainceDetailID { get; set; }
        public int ComplainceID { get; set; }
        public string ComplainceRef { get; set; }
        public string ComplianceTypeName { get; set; }
        public string DetailsOfComplianceReq { get; set; }
        public string TypeOfNonCompliance { get; set; }
        public string Remarks { get; set; }
        public string ActSection { get; set; }
        
        public string ClauseRef { get; set; }
        
        public string Period { get; set; }
        public int DeptId { get; set; }
        public string DepartmentName { get; set; }

        public int BUId { get; set; }
        public string BUName { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string CertificatePath { get; set; }
        public string FileName { get; set; }
        public DateTime ComplianceDueDate { get; set; }
    }
    public class InvalidRecords
    {
        public int ComplianceID { get; set; }
        public int BusinessUnitID { get; set; }
        public string BusinessUnitName { get; set; }
        public int ComplianceTypeID { get; set; }
        public string ComplianceTypeName { get; set; }
        public int DrivenById { get; set; }
        public string DrivenByName { get; set; }
        public string ComplianceArea { get; set; }
        public string ActSectionReference { get; set; }
        public string ComplianceRef { get; set; }
        public string ClauseRef { get; set; }
        public int ComplianceNatureID { get; set; }
        public string NatureOfComplianceName { get; set; }
        public int LawId { get; set; }
        public string LawName { get; set; }
        public string Territory { get; set; }
        public string Priority { get; set; }
        public int FrequencyID { get; set; }
        public string FrequencyName { get; set; }
        public int OccursEveryID { get; set; }
        public string OccursEveryName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime StandardDueDate { get; set; }
        public int DeptId { get; set; }
        public string DepartmentName { get; set; }
        public int InitiatorId { get; set; }
        public string InitiatorName { get; set; }
        public int ApproverId { get; set; }
        public string ApproverName { get; set; }
        public string FormsIfAny { get; set; }
        public int DueOnEvery { get; set; }
        public int SpecificGeneralId { get; set; }
        public string SpecificGeneral { get; set; }
        public string DetailsOfComplianceRequirements { get; set; }
        public string NonCompliancePenalty { get; set; }
        public string ActionsToBeTaken { get; set; }
        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }
        public string GovernmentLegislation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public int AssignedToID { get; set; }
        public string AssignedToName { get; set; }
        public int ComplianceStatusId { get; set; }
        public string ComplianceStatusName { get; set; }
        public string Remarks { get; set; }
        public DateTime FirstDueDate { get; set; }
        public string InvalidColumn { get; set; }
    }
    public class InvalidRecordss
    {
        public int RowNumber { get; set; }
        public string InvalidColumn { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ComplianceMasterDetailCombined
    {
        public int ComplianceID { get; set; }
        public string ClauseRef { get; set; }
        public string ComplianceArea { get; set; }
        public string ComplianceRef { get; set; }
        public int ComplianceTypeID { get; set; }
        public string ComplianceTypeName { get; set; }
        public string GovernmentLegislation { get; set; }
        public string ActSectionReference { get; set; }
        public int ComplianceNatureID { get; set; }
        public string NatureOfComplianceName { get; set; }
        public int FrequencyID { get; set; }
        public string FrequencyName { get; set; }
        public int DeptId { get; set; }
        public string DepartmentName { get; set; }
        public string SpecificGeneral { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime StandardDueDate { get; set; }
        public string DetailsOfComplianceRequirements { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public int ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public int AssignedToID { get; set; }
        public string AssignedToName { get; set; }
        public int ComplianceStatusId { get; set; }
        public string ComplianceStatusName { get; set; }
        public int ComplianceDetailID { get; set; }
        public string UserComments { get; set; }
        public DateTime UserCommentsUpdatedDate { get; set; }
        public string DocumentPath { get; set; }
        public DateTime MailSentOn { get; set; }
        public int ReminderNo { get; set; }
        public int Priority { get; set; }
        public string PriorityName { get; set; }
        public  List<DocumentLink> DocumentLink { get; set; }
        public DateTime AssignedDate { get; set; }
        public string ApproverComments { get; set; }
        public DateTime ApproveDate { get; set; }
        public DateTime RejectDate { get; set; }
        public char IsApproved { get; set; }
        public char IsRejected { get; set; }
        public string Remarks { get; set; }
        public int PreparerId { get; set; }
        public string PreparerName { get; set; }
        public string ApproverEmail { get; set; }
        public string UserEmail { get; set; }
    }
    public class ComplianceMasterDetails
    {
        public int ComplianceID { get; set; }
        public int ComplianceDetailID { get; set; }
        public string ComplianceArea { get; set; }
        public int ComplianceTypeID { get; set; }
        public string ComplianceTypeName { get; set; }
        public string GovernmentLegislation { get; set; }
        public string ActSectionReference { get; set; }
        public int ComplianceNatureID { get; set; }
        public string NatureOfComplianceName { get; set; }
        public int FrequencyID { get; set; }
        public string FrequencyName { get; set; }
        public int DeptId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime StandardDueDate { get; set; }
        public string DetailsOfComplianceRequirements { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public int ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public int AssignedToID { get; set; }
        public string AssignedToName { get; set; }
        public int ComplianceStatusId { get; set; }
        public string ComplianceStatusName { get; set; }
        public string UserComments { get; set; }
        public DateTime UserCommentsUpdatedDate { get; set; }
        public DateTime MailSentOn { get; set; }
        public DateTime AssignedDate { get; set; }
        public int ReminderNo { get; set; }
        public int Priority { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentsUploaded { get; set; }
        public string ApproverComments { get; set; }
        public DateTime ApproveDate { get; set; }
        public DateTime RejectDate { get; set; }
        public char IsApproved { get; set; }
        public char IsRejected { get; set; }
    }
    public class ComplianceAssigned
    {
        public int ComplianceID { get; set; }
        public int AssignedToDeptID { get; set; }
        public int AssignedToUserID { get; set; }
        public int AssignedByID { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public int ReminderNo { get; set; }
        public int ComplianceStatusID { get; set; }
        public DateTime EffectiveToDate { get; set; }
        public DateTime MailSentOn { get; set; }
        public string UserRemarks { get; set; }
        public int Priotity { get; set; }
        public int ApprovedByID { get; set; }
        public int CreatedByID { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedByID { get; set; }
        public bool IsActive { get; set; }
        public int DocumentLinkID { get; set; }
    }
    public class DocumentLink
    {
        public int DocumentLinkID { get; set; }
        public string DocumentLinkName { get; set; }
    }
    public class Creator
    {
        public int CreatorID { get; set; }
        public string CreatorName { get; set; }
    }
    public class Approver
    {
        public int ApproverID { get; set; }
        public string ApproverName { get; set; }
    }
    public class DocumentType
    {
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentDescription { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTill { get; set; }
    }
    public class ComplianceType
    {
        public int ComplianceTypeID { get; set; }
        public string ComplianceTypeName { get; set; }
        public string Description { get; set; }
    }
    public class Frequency
    {
        public int FrequencyID { get; set; }
        public string FrequencyName { get; set; }
        public string Description { get; set; }
    }
    public class Period
    {
        public int Id { get; set; }
        public string PeriodName { get; set; }
    }
    public class ComplianceStatuss
    {
        public int ComplianceStatusID { get; set; }
        public string ComplianceStatusName { get; set; }
    }
    public class ComplianceStatus
    {
        public int ComplianceStatusID { get; set; }
        public string ComplianceStatusName { get; set; }
    }
    public class ComplianceDocuments
    {
        public int DocumentID { get; set; }
        public int ComplianceID { get; set; }
        public string ComplianceType { get; set; }
        public string ComplianceRef { get; set; }
        public string ComplianceNature { get; set; }
        public string Priority { get; set; }
        public string Department { get; set; }
        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedDate { get; set; }
        public string DownloadPath { get; set; }
    }
    public class Notification
    {
        public int NotificationId { get; set; }
        public string NotificationMessage { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationType { get; set; }
        public int UserId { get; set; }
        public int ComplianceDetailId { get; set; }
        public string ComplianceRef { get; set; }
    }
    public class SideMenu
    {
        public int MenuId { get; set; }
        public string MenuText { get; set; }
        public string MenuUrl { get; set; }
        public int? ParentMenuId { get; set; }
    }
}