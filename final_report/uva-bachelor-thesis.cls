\NeedsTeXFormat{LaTeX2e}
\ProvidesClass{uva-bachelor-thesis}[2005/06/10 Version 0.1 Rein van den Boomgaard]
\RequirePackage{graphicx}


\def\supervisors#1{\gdef\@supervisors{#1}}
\def\signedby#1{\gdef\@signedby{#1}}


\DeclareOption*{\PassOptionsToClass{\CurrentOption}{report}}
\ProcessOptions
\LoadClass[10pt,twoside]{report}

\usepackage[a4paper,scale={0.7,0.8},twoside]{geometry}

\usepackage[sf,bf]{titlesec}
\titleformat{\chapter}[display]
{\sffamily\raggedleft}
{\vspace{0.25pc}\Large \MakeUppercase{\chaptertitlename}\ \thechapter}
{0.25pc}
{\Huge\bfseries}
[\vspace{0.5pc}\titlerule]

\usepackage[svgnames]{xcolor}
\usepackage[absolute,overlay]{textpos}
\usepackage{rotating}
\renewcommand{\maketitle}{%
	\thispagestyle{empty}
	\hspace*{1em} 	% a fake element on the page (else texpos absolute option
			% doesn't work)

	\begin{textblock*}{10cm}(6cm,3cm)
		\noindent\large\scshape BSc. Computer Science\\[2ex]
		\includegraphics[height=1cm]{uva_en}
		%\Large Universiteit van Amsterdam
	\end{textblock*}

	\begin{textblock*}{10cm}(6cm,12cm)
		\noindent
		\sffamily\Huge \@title
	\end{textblock*}

	\begin{textblock*}{10cm}(6cm,18cm)
		\noindent
		\sffamily\Large \@author\\[3pc]
		\@date
	\end{textblock*}

	\begin{textblock*}{10cm}(6cm,26cm)
		\noindent\sffamily
		\begin{description}
		\item[\sffamily Supervisor(s):] \@supervisors
		\item[\sffamily Signed:] \@signedby
		\end{description}
	\end{textblock*}
	
	\textblockcolor{LightGray}
	\begin{textblock*}{2cm}(3cm,1cm)
		\vspace*{26cm}\hspace*{1em}
	\end{textblock*}

	\begin{textblock*}{2cm}(4.2cm,25cm)
		\begin{rotate}{90}
			\noindent\scshape\Huge\textcolor{Grey}{
				Computer Science --- University of Amsterdam}
		\end{rotate}
	\end{textblock*}
	\cleardoublepage
}


\endinput
