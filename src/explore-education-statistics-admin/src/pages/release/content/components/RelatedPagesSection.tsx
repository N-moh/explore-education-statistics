import EditableLink from '@admin/components/editable/EditableLink';
import Link from '@admin/components/Link';
import { useEditingContext } from '@admin/contexts/EditingContext';
import releaseContentRelatedInformationService from '@admin/services/releaseContentRelatedInformationService';
import { EditableRelease } from '@admin/services/releaseContentService';
import Button from '@common/components/Button';
import ButtonText from '@common/components/ButtonText';
import {
  Form,
  FormFieldset,
  FormFieldTextInput,
} from '@common/components/form';
import { BasicLink } from '@common/services/publicationService';
import Yup from '@common/validation/yup';
import { Formik } from 'formik';
import React, { useState } from 'react';

interface Props {
  release: EditableRelease;
}

const RelatedPagesSection = ({ release }: Props) => {
  const [links, setLinks] = useState<BasicLink[]>(release.relatedInformation);
  const [formOpen, setFormOpen] = useState<boolean>(false);

  const { isEditing } = useEditingContext();

  const addLink = (link: Omit<BasicLink, 'id'>) => {
    return new Promise(resolve => {
      releaseContentRelatedInformationService
        .create(release.id, link)
        .then(newLinks => {
          setLinks(newLinks);
          resolve();
        });
    });
  };

  const removeLink = (linkId: string) => {
    releaseContentRelatedInformationService
      .delete(release.id, linkId)
      .then(setLinks);
  };

  const renderLinkForm = () => {
    return !formOpen ? (
      <Button onClick={() => setFormOpen(true)}>Add related page link</Button>
    ) : (
      <Formik<Omit<BasicLink, 'id'>>
        initialValues={{ description: '', url: '' }}
        validationSchema={Yup.object({
          description: Yup.string().required('Enter a link title'),
          url: Yup.string()
            .url('Enter a valid link url')
            .required('Enter a link url'),
        })}
        onSubmit={link =>
          addLink(link).then(() => {
            setFormOpen(false);
          })
        }
      >
        {form => {
          return (
            <Form {...form} id="relatedPageForm">
              <FormFieldset
                id="relatedLink"
                legend="Add related page link"
                legendSize="m"
              >
                <FormFieldTextInput label="Title" name="description" />
                <FormFieldTextInput label="Link url" name="url" />
              </FormFieldset>
              <Button
                type="submit"
                className="govuk-button govuk-!-margin-right-1"
              >
                Create link
              </Button>
              <ButtonText
                className="govuk-button govuk-button--secondary"
                onClick={() => {
                  form.resetForm();
                  setFormOpen(false);
                }}
              >
                Cancel
              </ButtonText>
            </Form>
          );
        }}
      </Formik>
    );
  };

  return (
    <>
      {(isEditing || links.length > 0) && (
        <>
          <h2
            className="govuk-heading-s govuk-!-margin-top-6"
            id="related-pages"
          >
            Related pages
          </h2>
          <nav role="navigation" aria-labelledby="related-content">
            <ul className="govuk-list">
              {links.map(({ id, description, url }) => (
                <li key={id}>
                  <EditableLink removeOnClick={() => removeLink(id)} to={url}>
                    {description}
                  </EditableLink>
                </li>
              ))}
            </ul>
          </nav>
        </>
      )}
      {isEditing && renderLinkForm()}
    </>
  );
};

export default RelatedPagesSection;