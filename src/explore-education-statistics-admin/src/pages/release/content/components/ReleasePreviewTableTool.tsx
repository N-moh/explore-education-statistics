import LoadingSpinner from '@common/components/LoadingSpinner';
import useAsyncHandledRetry from '@common/hooks/useAsyncHandledRetry';
import dataBlockService from '@admin/services/dataBlockService';
import TableToolWizard, {
  InitialTableToolState,
} from '@common/modules/table-tool/components/TableToolWizard';
import WizardStep from '@common/modules/table-tool/components/WizardStep';
import WizardStepHeading from '@common/modules/table-tool/components/WizardStepHeading';
import mapFullTable from '@common/modules/table-tool/utils/mapFullTable';
import mapTableHeadersConfig from '@common/modules/table-tool/utils/mapTableHeadersConfig';
import tableBuilderService from '@common/services/tableBuilderService';
import ButtonText from '@common/components/ButtonText';
import ReleasePreviewTableToolFinalStep from '@admin/pages/release/content/components/ReleasePreviewTableToolFinalStep';
import { BasicPublicationDetails } from '@admin/services/publicationService';
import { Publication } from '@common/services/publicationService';
import React, { useState } from 'react';

interface Props {
  releaseId: string;
  publication: Publication | BasicPublicationDetails;
}
const ReleasePreviewTableTool = ({ releaseId, publication }: Props) => {
  const [highlightId, setHighlightId] = useState<string>();

  const { value: initialState, isLoading } = useAsyncHandledRetry<
    InitialTableToolState | undefined
  >(async () => {
    const {
      highlights,
      subjects,
    } = await tableBuilderService.getReleaseSubjectsAndHighlights(releaseId);

    highlights.filter(highlight => highlight.id !== highlightId);

    if (highlightId) {
      const { table, query } = await dataBlockService.getDataBlock(highlightId);

      const [subjectMeta, tableData] = await Promise.all([
        tableBuilderService.getSubjectMeta(query.subjectId),
        tableBuilderService.getTableData({
          releaseId,
          ...query,
        }),
      ]);

      const fullTable = mapFullTable(tableData);
      const tableHeaders = mapTableHeadersConfig(table.tableHeaders, fullTable);

      return {
        initialStep: 5,
        subjects,
        highlights,
        query: {
          ...query,
          publicationId: publication.id,
          releaseId,
        },
        subjectMeta,
        response: {
          table: fullTable,
          tableHeaders,
        },
      };
    }

    return {
      initialStep: 1,
      subjects,
      highlights,
      query: {
        publicationId: publication.id,
        releaseId,
        subjectId: '',
        indicators: [],
        filters: [],
        locations: {},
      },
    };
  }, [releaseId, highlightId]);

  return (
    <LoadingSpinner loading={isLoading}>
      {initialState && (
        <>
          <h2>Table tool</h2>

          <TableToolWizard
            themeMeta={[]}
            hidePublicationSelectionStage
            initialState={initialState}
            onSubjectStepBack={() => setHighlightId(undefined)}
            renderHighlightLink={highlight => (
              <ButtonText
                onClick={() => {
                  setHighlightId(highlight.id);
                }}
              >
                {highlight.name}
              </ButtonText>
            )}
            finalStep={({ response, query }) => (
              <WizardStep>
                {wizardStepProps => (
                  <>
                    <WizardStepHeading {...wizardStepProps}>
                      Explore data
                    </WizardStepHeading>

                    {query && response && (
                      <ReleasePreviewTableToolFinalStep
                        publication={publication as BasicPublicationDetails}
                        table={response.table}
                        tableHeaders={response.tableHeaders}
                      />
                    )}
                  </>
                )}
              </WizardStep>
            )}
          />
        </>
      )}
    </LoadingSpinner>
  );
};

export default ReleasePreviewTableTool;