import {
  Form,
  FormFieldCheckboxGroup,
  FormFieldset,
} from '@common/components/form';
import { CheckboxOption } from '@common/components/form/FormCheckboxGroup';
import { InjectedWizardProps } from '@common/modules/table-tool/components/Wizard';
import WizardStepHeading from '@common/modules/table-tool/components/WizardStepHeading';
import WizardStepFormActions from '@common/modules/table-tool/components/WizardStepFormActions';
import ResetFormOnPreviousStep from '@common/modules/table-tool/components/ResetFormOnPreviousStep';
import { FileInfo } from '@common/services/types/file';
import { Release } from '@common/services/publicationService';
import Yup from '@common/validation/yup';
import SummaryList from '@common/components/SummaryList';
import SummaryListItem from '@common/components/SummaryListItem';
import useFormSubmit from '@common/hooks/useFormSubmit';
import Details from '@common/components/Details';
import { Subject } from '@common/services/tableBuilderService';
import ContentHtml from '@common/components/ContentHtml';
import Tag from '@common/components/Tag';
import React, { useMemo } from 'react';
import { Formik } from 'formik';

interface DownloadFormValues {
  files: string[];
}
export type DownloadFormSubmitHandler = (values: { files: string[] }) => void;

export interface SubjectWithDownloadFiles extends Subject {
  downloadFile: FileInfo;
}

interface Props {
  release?: Release;
  subjects: SubjectWithDownloadFiles[];
  initialValues?: { files: string[] };
  onSubmit: DownloadFormSubmitHandler;
}

const DownloadStep = ({
  release,
  subjects,
  initialValues = { files: [] },
  onSubmit,
  ...stepProps
}: Props & InjectedWizardProps) => {
  const { isActive, currentStep, stepNumber } = stepProps;

  const stepEnabled = currentStep > stepNumber;
  const stepHeading = (
    <WizardStepHeading {...stepProps} fieldsetHeading stepEnabled={stepEnabled}>
      <span className="dfe-flex dfe-align-items--center">
        Choose files to download{' '}
        {release && release.latestRelease ? (
          <Tag strong className="govuk-!-margin-left-4">
            This is the latest data
          </Tag>
        ) : (
          <Tag strong colour="orange" className="govuk-!-margin-left-4">
            This is not the latest data
          </Tag>
        )}
      </span>
    </WizardStepHeading>
  );

  const getTimePeriod = (subject: Subject) => {
    const { from, to } = subject.timePeriods;

    if (from && to) {
      return from === to ? from : `${from} to ${to}`;
    }

    return from || to;
  };

  const checkboxOptions = useMemo<CheckboxOption[]>(
    () =>
      subjects.map(subject => {
        const { content } = subject;
        const geographicLevels = [...subject.geographicLevels]
          .sort()
          .join('; ');
        const timePeriod = getTimePeriod(subject);
        const hasDetails = content || geographicLevels || timePeriod;

        return {
          label: `${subject.name} (${subject.downloadFile.extension}, ${subject.downloadFile.size})`,
          value: subject.downloadFile.id,
          hint: hasDetails ? (
            <Details summary="More details" className="govuk-!-margin-bottom-2">
              <h4>This subject includes the following data:</h4>
              <SummaryList>
                {geographicLevels && (
                  <SummaryListItem term="Geographic levels">
                    {geographicLevels}
                  </SummaryListItem>
                )}

                {timePeriod && (
                  <SummaryListItem term="Time period">
                    {timePeriod}
                  </SummaryListItem>
                )}

                {content && (
                  <SummaryListItem term="Content">
                    <ContentHtml html={content} />
                  </SummaryListItem>
                )}
              </SummaryList>
            </Details>
          ) : null,
        };
      }),
    [subjects],
  );

  const handleSubmit = useFormSubmit<DownloadFormValues>(onSubmit);

  return (
    <Formik<DownloadFormValues>
      enableReinitialize
      initialValues={initialValues}
      validateOnBlur={false}
      validationSchema={Yup.object<DownloadFormValues>({
        files: Yup.array().of(Yup.string()).required('Choose a file'),
      })}
      onSubmit={handleSubmit}
    >
      {form => {
        return isActive ? (
          <Form id="downloadForm" showSubmitError>
            <FormFieldset id="downloadFiles" legend={stepHeading}>
              {checkboxOptions.length > 0 && (
                <FormFieldCheckboxGroup<DownloadFormValues>
                  name="files"
                  legend="Choose files from the list below"
                  legendHidden
                  disabled={form.isSubmitting}
                  options={checkboxOptions}
                />
              )}
            </FormFieldset>

            {checkboxOptions.length > 0 ? (
              <WizardStepFormActions
                {...stepProps}
                submitText="Download selected files"
                submittingText="Downloading"
              />
            ) : (
              <p>No downloads available.</p>
            )}
          </Form>
        ) : (
          <ResetFormOnPreviousStep
            currentStep={currentStep}
            stepNumber={stepNumber}
          />
        );
      }}
    </Formik>
  );
};

export default DownloadStep;