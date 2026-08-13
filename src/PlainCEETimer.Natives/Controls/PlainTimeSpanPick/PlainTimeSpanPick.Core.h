#pragma once

#define PTSPSEG_LITERAL             0
#define PTSPSEG_DAYS                1
#define PTSPSEG_HOURS               2
#define PTSPSEG_MINUTES             3
#define PTSPSEG_SECONDS             4
#define PTSPSEG_NUM_MIN             PTSPSEG_DAYS
#define PTSPSEG_NUM_MAX             PTSPSEG_SECONDS

#define PTSP_NUMERIC_FORMAT         L"%lld"

#define QUOTE                       L'\''

#define FreeCore()					HEAPFREE(m_lpLiterals); HEAPFREE(m_lpSegments)
