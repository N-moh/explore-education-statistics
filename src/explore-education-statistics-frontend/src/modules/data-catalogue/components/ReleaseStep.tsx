import SummaryList from '@common/components/SummaryList';
import SummaryListItem from '@common/components/SummaryListItem';
import { InjectedWizardProps } from '@common/modules/table-tool/components/Wizard';
import WizardStepHeading from '@common/modules/table-tool/components/WizardStepHeading';
import WizardStepEditButton from '@common/modules/table-tool/components//WizardStepEditButton';
import { Release } from '@common/services/publicationService';
import ReleaseForm, {
  ReleaseFormSubmitHandler,
} from '@frontend/modules/data-catalogue/components/ReleaseForm';
import React from 'react';

interface Props {
  releases: Release[];
  selectedRelease?: Release;
  onSubmit: ReleaseFormSubmitHandler;
}

const ReleaseStep = ({
  releases,
  selectedRelease,
  onSubmit,
  ...stepProps
}: Props & InjectedWizardProps) => {
  const { isActive, currentStep, stepNumber } = stepProps;

  const stepEnabled = currentStep > stepNumber;
  const stepHeading = (
    <WizardStepHeading {...stepProps} fieldsetHeading stepEnabled={stepEnabled}>
      Choose a release
    </WizardStepHeading>
  );

  if (isActive) {
    return (
      <ReleaseForm
        {...stepProps}
        options={releases}
        onSubmit={onSubmit}
        legendSize="l"
        legend={stepHeading}
      />
    );
  }

  return (
    <div className="govuk-grid-row">
      <div className="govuk-grid-column-two-thirds">
        {stepHeading}
        <SummaryList noBorder>
          <SummaryListItem term="Release">
            {selectedRelease?.title}
          </SummaryListItem>
        </SummaryList>
      </div>
      <div className="govuk-grid-column-one-third dfe-align--right">
        {stepEnabled && (
          <WizardStepEditButton {...stepProps} editTitle="Change release" />
        )}
      </div>
    </div>
  );
};

export default ReleaseStep;