import Details from '@common/components/Details';
import { Form, FormFieldRadioGroup } from '@common/components/form';
import { FormFieldsetProps } from '@common/components/form/FormFieldset';
import { RadioOption } from '@common/components/form/FormRadioGroup';
import ContentHtml from '@common/components/ContentHtml';
import SummaryList from '@common/components/SummaryList';
import SummaryListItem from '@common/components/SummaryListItem';
import ResetFormOnPreviousStep from '@common/modules/table-tool/components/ResetFormOnPreviousStep';
import { Subject } from '@common/services/tableBuilderService';
import Yup from '@common/validation/yup';
import { Formik } from 'formik';
import React, { ReactNode, useMemo } from 'react';
import { InjectedWizardProps } from './Wizard';
import WizardStepFormActions from './WizardStepFormActions';

export interface SubjectFormValues {
  subjectId: string;
}

export type SubjectFormSubmitHandler = (values: { subjectId: string }) => void;

const formId = 'publicationSubjectForm';

interface Props {
  legend?: ReactNode;
  legendSize?: FormFieldsetProps['legendSize'];
  initialValues?: { subjectId: string };
  onSubmit: SubjectFormSubmitHandler;
  options: Subject[];
}

const SubjectForm = ({
  goToNextStep,
  legend,
  legendSize = 'l',
  initialValues = {
    subjectId: '',
  },
  onSubmit,
  options,
  ...stepProps
}: Props & InjectedWizardProps) => {
  const { isActive, currentStep, stepNumber } = stepProps;

  const getTimePeriod = (subject: Subject) => {
    const { from, to } = subject.timePeriods;

    if (from && to) {
      return from === to ? from : `${from} to ${to}`;
    }

    return from || to;
  };

  const radioOptions = useMemo<RadioOption[]>(
    () =>
      options.map(option => {
        const { content } = option;
        const geographicLevels = [...option.geographicLevels].sort().join('; ');

        const timePeriod = getTimePeriod(option);

        const hasDetails = content || geographicLevels || timePeriod;

        return {
          label: option.name,
          value: option.id,
          hint: hasDetails ? (
            <Details
              summary="More details"
              className="govuk-!-margin-bottom-2 govuk-!-margin-top-2"
            >
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
    [options],
  );
  return (
    <Formik<SubjectFormValues>
      enableReinitialize
      initialValues={initialValues}
      validateOnBlur={false}
      validationSchema={Yup.object<SubjectFormValues>({
        subjectId: Yup.string().required('Choose a subject'),
      })}
      onSubmit={async ({ subjectId }) => {
        await onSubmit({
          subjectId,
        });
        goToNextStep();
      }}
    >
      {form => {
        return isActive ? (
          <Form {...form} id={formId} showSubmitError>
            <FormFieldRadioGroup<SubjectFormValues>
              name="subjectId"
              legend={legend}
              legendSize={legendSize}
              disabled={form.isSubmitting}
              options={radioOptions}
            />

            {radioOptions.length > 0 ? (
              <WizardStepFormActions {...stepProps} />
            ) : (
              <p>No subjects available.</p>
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

export default SubjectForm;
