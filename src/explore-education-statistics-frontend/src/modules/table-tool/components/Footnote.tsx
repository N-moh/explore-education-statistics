import React from 'react';
import { FullTableMeta } from '@common/modules/full-table/types/fullTable';
import Details from '@common/components/Details';

export interface FootnoteProps {
  content: FullTableMeta['footnotes'];
}

const Footnote = ({ content }: FootnoteProps) => {
  const footnoteList = content.map(function createFootnotes(footnotes, index) {
    return (
      <li key={footnotes.id}>
        <span className="govuk-!-font-weight-bold">{index + 1}. </span>
        {footnotes.label}
      </li>
    );
  });
  return (
    <ol className="govuk-list">
      {footnoteList.slice(0, 3)}
      {footnoteList.length > 3 && (
        <Details summary="View additional footnotes">
          <ol className="govuk-list">{footnoteList.slice(3)}</ol>
        </Details>
      )}
    </ol>
  );
};

export default Footnote;
