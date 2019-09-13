import Link from '@admin/components/Link';
import Page from '@admin/components/Page';
import dashboardRoutes from '@admin/routes/dashboard/routes';
import { ContactDetails, IdTitlePair } from '@admin/services/common/types';
import service from '@admin/services/edit-publication/service';
import Button from '@common/components/Button';
import { FormFieldset, Formik } from '@common/components/form';
import Form from '@common/components/form/Form';
import FormFieldRadioGroup from '@common/components/form/FormFieldRadioGroup';
import FormFieldSelect from '@common/components/form/FormFieldSelect';
import FormFieldTextInput from '@common/components/form/FormFieldTextInput';
import handleServerSideValidation, {
  errorCodeToFieldError,
} from '@common/components/form/util/serverValidationHandler';
import RelatedInformation from '@common/components/RelatedInformation';
import SummaryList from '@common/components/SummaryList';
import SummaryListItem from '@common/components/SummaryListItem';
import Yup from '@common/lib/validation/yup';
import { FormikProps } from 'formik';
import orderBy from 'lodash/orderBy';
import React, { useEffect, useState } from 'react';
import { RouteComponentProps } from 'react-router';

interface MatchProps {
  topicId: string;
}

interface FormValues {
  publicationTitle: string;
  methodologyChoice?: 'existing' | 'new';
  selectedMethodologyId?: string;
  selectedContactId: string;
}

const serverSideValidationHandler = handleServerSideValidation(
  errorCodeToFieldError(
    'SLUG_NOT_UNIQUE',
    'publicationTitle',
    'Choose a unique title',
  ),
);

interface CreatePublicationModel {
  methodologies: IdTitlePair[];
  contacts: ContactDetails[];
}

const CreatePublicationPage = ({
  match,
  history,
}: RouteComponentProps<MatchProps>) => {
  const { topicId } = match.params;

  const [model, setModel] = useState<CreatePublicationModel>();

  useEffect(() => {
    Promise.all([
      service.getMethodologies(),
      service.getPublicationAndReleaseContacts(),
    ]).then(([methodologies, contacts]) => {
      setModel({
        methodologies,
        contacts,
      });
    });
  }, []);

  const submitFormHandler = async (values: FormValues) => {
    await service.createPublication({
      topicId,
      ...values,
    });

    history.push(dashboardRoutes.adminDashboard);
  };

  const cancelHandler = () => {
    history.push(dashboardRoutes.adminDashboard);
  };

  const getSelectedContact = (
    contactId: string,
    availableContacts: ContactDetails[],
  ) =>
    availableContacts.find(contact => contact.id === contactId) ||
    availableContacts[0];

  const formId = 'createPublicationForm';

  return (
    <Page
      wide
      breadcrumbs={[
        {
          name: 'Create new publication',
        },
      ]}
    >
      <div className="govuk-grid-row">
        <div className="govuk-grid-column-two-thirds">
          <h1 className="govuk-heading-xl">
            <span className="govuk-caption-xl">Topic name here</span>
            Create new publication
          </h1>
        </div>
        <div className="govuk-grid-column-one-third">
          <RelatedInformation heading="Help and guidance">
            <ul className="govuk-list">
              <li>
                <Link
                  to="/prototypes/documentation/create-new-publication"
                  target="blank"
                >
                  Creating a new publication{' '}
                </Link>
              </li>
            </ul>
          </RelatedInformation>
        </div>
      </div>
      {model && (
        <Formik<FormValues>
          enableReinitialize
          initialValues={{
            publicationTitle: '',
            selectedContactId: orderBy(
              model.contacts,
              contact => contact.contactName,
            )[0].id,
            methodologyChoice: undefined,
            selectedMethodologyId: orderBy(
              model.methodologies,
              methodology => methodology.title,
            )[0].id,
          }}
          validationSchema={Yup.object<FormValues>({
            publicationTitle: Yup.string().required(
              'Enter a publication title',
            ),
            selectedContactId: Yup.string().required(
              'Choose a publication and release contact',
            ),
            methodologyChoice: Yup.mixed().required('Choose a methodology'),
            selectedMethodologyId: Yup.string().when('methodologyChoice', {
              is: 'existing',
              then: Yup.string().required('Choose a methodology'),
              otherwise: Yup.string(),
            }),
          })}
          onSubmit={submitFormHandler}
          render={(form: FormikProps<FormValues>) => {
            return (
              <Form
                id={formId}
                submitValidationHandler={serverSideValidationHandler}
              >
                <FormFieldTextInput
                  id={`${formId}-publicationTitle`}
                  label="Enter publication title"
                  name="publicationTitle"
                />
                <FormFieldRadioGroup
                  id={`${formId}-methodologyChoice`}
                  legend="Choose a methodology for this publication"
                  name="methodologyChoice"
                  options={[
                    {
                      value: 'existing',
                      label: 'Add existing methodology',
                    },
                    {
                      value: 'new',
                      label: 'Create new methodology',
                    },
                  ]}
                />
                {form.values.methodologyChoice === 'existing' && (
                  <FormFieldSelect
                    id={`${formId}-selectedMethodologyId`}
                    name="selectedMethodologyId"
                    label="Select methodology"
                    options={model.methodologies.map(methodology => ({
                      label: methodology.title,
                      value: methodology.id,
                    }))}
                  />
                )}
                <FormFieldset
                  id={`${formId}-selectedContactIdFieldset`}
                  legend="Choose the contact for this publication"
                  hint="They will be the main point of contact for data and methodology enquiries for this publication and its releases."
                >
                  <FormFieldSelect
                    id={`${formId}-selectedContactId`}
                    label="Publication and release contact"
                    name="selectedContactId"
                    options={model.contacts.map(contact => ({
                      label: contact.contactName,
                      value: contact.id,
                    }))}
                  />
                </FormFieldset>
                {form.values.selectedContactId && (
                  <SummaryList>
                    <SummaryListItem term="Email">
                      {
                        getSelectedContact(
                          form.values.selectedContactId,
                          model.contacts,
                        ).teamEmail
                      }
                    </SummaryListItem>
                    <SummaryListItem term="Telephone">
                      {
                        getSelectedContact(
                          form.values.selectedContactId,
                          model.contacts,
                        ).contactTelNo
                      }
                    </SummaryListItem>
                  </SummaryList>
                )}
                <Button type="submit" className="govuk-!-margin-top-6">
                  Create publication
                </Button>
                <div className="govuk-!-margin-top-6">
                  <Link to="#" onClick={cancelHandler}>
                    Cancel publication
                  </Link>
                </div>
              </Form>
            );
          }}
        />
      )}
    </Page>
  );
};

export default CreatePublicationPage;
