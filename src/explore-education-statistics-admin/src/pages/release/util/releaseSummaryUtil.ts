import {
  dayMonthYearInputsToDate,
  dayMonthYearInputsToValues,
  IdTitlePair,
  TimePeriodCoverageGroup,
} from '@admin/services/common/types';
import {ReleaseStatus} from "@admin/services/dashboard/types";
import { CreateReleaseRequest } from '@admin/services/release/create-release/types';
import { UpdateReleaseSummaryDetailsRequest } from '@admin/services/release/edit-release/summary/types';
import { BaseReleaseSummaryDetailsRequest } from '@admin/services/release/types';
import { FormValues as CreateFormValues } from '../create-release/CreateReleasePage';
import { EditFormValues } from '../summary/ReleaseSummaryForm';

export const assembleBaseReleaseSummaryRequestFromForm = (
  values: EditFormValues,
): BaseReleaseSummaryDetailsRequest => {
  return {
    timePeriodCoverage: {
      value: values.timePeriodCoverageCode,
    },
    releaseName: parseInt(values.timePeriodCoverageStartYear, 10),
    publishScheduled: dayMonthYearInputsToDate(values.scheduledPublishDate),
    nextReleaseDate: dayMonthYearInputsToValues(values.nextReleaseDate),
    typeId: values.releaseTypeId,
  };
};

export const assembleCreateReleaseRequestFromForm = (
  publicationId: string,
  values: CreateFormValues,
): CreateReleaseRequest => {
  return {
    publicationId,
    templateReleaseId:
      values.templateReleaseId !== 'new' ? values.templateReleaseId : '',
    ...assembleBaseReleaseSummaryRequestFromForm(values),
  };
};

export const assembleUpdateReleaseSummaryRequestFromForm = (
  releaseId: string,
  values: EditFormValues,
): UpdateReleaseSummaryDetailsRequest => {
  return {
    releaseId,
    ...assembleBaseReleaseSummaryRequestFromForm(values),
  };
};

export const findTimePeriodCoverageGroup = (
  code: string,
  timePeriodCoverageGroups: TimePeriodCoverageGroup[],
) => {
  return (
    timePeriodCoverageGroups.find(group =>
      group.timeIdentifiers
        .map(option => option.identifier.value)
        .some(id => id === code),
    ) || timePeriodCoverageGroups[0]
  );
};

export const findTimePeriodCoverageOption = (
  code: string,
  timePeriodCoverageGroups: TimePeriodCoverageGroup[],
) =>
  timePeriodCoverageGroups
    .flatMap(group => group.timeIdentifiers)
    .find(option => option.identifier.value === code) ||
  timePeriodCoverageGroups[0].timeIdentifiers[0];

export const getSelectedTimePeriodCoverageLabel = (
  timePeriodCoverageCode: string,
  timePeriodCoverageGroups: TimePeriodCoverageGroup[],
) =>
  findTimePeriodCoverageOption(timePeriodCoverageCode, timePeriodCoverageGroups)
    .identifier.label;

export const getSelectedReleaseType = (
  releaseTypeId: string,
  availableReleaseTypes: IdTitlePair[],
) =>
  availableReleaseTypes.find(type => type.id === releaseTypeId) ||
  availableReleaseTypes[0];

export const getReleaseStatusLabel = (approvalStatus: ReleaseStatus) => {
  switch (approvalStatus) {
    case 'Draft':
      return 'Draft';
    case 'HigherLevelReview':
      return 'In Review';
    case 'Approved':
      return 'Approved for Publication';
    default:
      return undefined;
  }
};

export default {};
