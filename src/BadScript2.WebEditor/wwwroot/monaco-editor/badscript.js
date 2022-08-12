function CreateAutoComplete()
{
	var libSource = [
		'declare class Facts {',
		'    /**',
		'     * Returns the next fact',
		'     */',
		'    static next():string',
		'}'
	].join('\n');
	var libUri = 'ts:filename/facts.d.ts';
	monaco.languages.badscript.javascriptDefaults.addExtraLib(libSource, libUri);


}

function AddBadScriptLanguage()
{
	monaco.languages.register({id: "badscript", extensions: [".bs"]});

	


	monaco.languages.setMonarchTokensProvider('badscript', {
		// Set defaultToken to invalid to see what you do not tokenize yet
		defaultToken: 'invalid',
		tokenPostfix: '.bs',

		keywords: [
			'as',
			'break',
			'catch',
			'class',
			'continue',
			'const',
			'else',
			'false',
			'for',
			'from',
			'function',
			'if',
			'in',
			'let',
			'new',
			'null',
			'return',
			'this',
			'throw',
			'true',
			'try',
			'while',
			'num',
			'string',
			'bool',
			'Array',
			'Function',
			'Table'
		],

		operators: [
			'<=',
			'>=',
			'==',
			'!=',
			'===',
			'!==',
			'=>',
			'+',
			'-',
			'**',
			'*',
			'/',
			'%',
			'++',
			'--',
			'<<',
			'</',
			'>>',
			'>>>',
			'&',
			'|',
			'^',
			'!',
			'~',
			'&&',
			'||',
			'??',
			'?',
			':',
			'=',
			'+=',
			'-=',
			'*=',
			'**=',
			'/=',
			'%=',
			'<<=',
			'>>=',
			'>>>=',
			'&=',
			'|=',
			'^=',
			'@'
		],

		// we include these common regular expressions
		symbols: /[=><!~?:&|+\-*\/^%]+/,
		escapes: /\\(?:[abfnrtv\\"']|x[\dA-Fa-f]{1,4}|u[\dA-Fa-f]{4}|U[\dA-Fa-f]{8})/,
		digits: /\d+(_+\d+)*/,
		octaldigits: /[0-7]+(_+[0-7]+)*/,
		binarydigits: /[0-1]+(_+[0-1]+)*/,
		hexdigits: /[[\da-fA-F]+(_+[\da-fA-F]+)*/,

		regexpctl: /[(){}\[\]$^|\-*+?.]/,
		regexpesc: /\\(?:[bBdDfnrstvwW0\\\/]|@regexpctl|c[A-Z]|x[\da-fA-F]{2}|u[\da-fA-F]{4})/,

		// The main tokenizer for our languages
		tokenizer: {
			root: [[/[{}]/, 'delimiter.bracket'], { include: 'common' }],

			common: [
				// identifiers and keywords
				[
					/[a-z_$][\w$]*/,
					{
						cases: {
							'@keywords': 'keyword',
							'@default': 'identifier'
						}
					}
				],
				[/[A-Z][\w$]*/, 'type.identifier'], // to show class names nicely
				// [/[A-Z][\w\$]*/, 'identifier'],

				// whitespace
				{ include: '@whitespace' },

				// regular expression: ensure it is terminated before beginning (otherwise it is an opeator)
				[
					/\/(?=([^\\\/]|\\.)+\/([dgimsuy]*)(\s*)(\.|;|,|\)|]|}|$))/,
					{ token: 'regexp', bracket: '@open', next: '@regexp' }
				],

				// delimiters and operators
				[/[()\[\]]/, '@brackets'],
				[/[<>](?!@symbols)/, '@brackets'],
				[/!(?=([^=]|$))/, 'delimiter'],
				[
					/@symbols/,
					{
						cases: {
							'@operators': 'delimiter',
							'@default': ''
						}
					}
				],

				// numbers
				[/(@digits)[eE]([\-+]?(@digits))?/, 'number.float'],
				[/(@digits)\.(@digits)([eE][\-+]?(@digits))?/, 'number.float'],
				[/0[xX](@hexdigits)n?/, 'number.hex'],
				[/0[oO]?(@octaldigits)n?/, 'number.octal'],
				[/0[bB](@binarydigits)n?/, 'number.binary'],
				[/(@digits)n?/, 'number'],

				// delimiter: after number because of .\d floats
				[/[;,.]/, 'delimiter'],

				// strings
				[/"([^"\\]|\\.)*$/, 'string.invalid'], // non-teminated string
				[/'([^'\\]|\\.)*$/, 'string.invalid'], // non-teminated string
				[/"/, 'string', '@string_double'],
				[/'/, 'string', '@string_single'],
				[/`/, 'string', '@string_backtick']
			],

			whitespace: [
				[/[ \t\r\n]+/, ''],
				[/\/\*\*(?!\/)/, 'comment.doc', '@jsdoc'],
				[/\/\*/, 'comment', '@comment'],
				[/\/\/.*$/, 'comment']
			],

			comment: [
				[/[^\/*]+/, 'comment'],
				[/\*\//, 'comment', '@pop'],
				[/[\/*]/, 'comment']
			],

			jsdoc: [
				[/[^\/*]+/, 'comment.doc'],
				[/\*\//, 'comment.doc', '@pop'],
				[/[\/*]/, 'comment.doc']
			],

			// We match regular expression quite precisely
			regexp: [
				[
					/(\{)(\d+(?:,\d*)?)(})/,
					['regexp.escape.control', 'regexp.escape.control', 'regexp.escape.control']
				],
				[
					/(\[)(\^?)(?=(?:[^]\\\/]|\\.)+)/,
					['regexp.escape.control', { token: 'regexp.escape.control', next: '@regexrange' }]
				],
				[/(\()(\?:|\?=|\?!)/, ['regexp.escape.control', 'regexp.escape.control']],
				[/[()]/, 'regexp.escape.control'],
				[/@regexpctl/, 'regexp.escape.control'],
				[/[^\\\/]/, 'regexp'],
				[/@regexpesc/, 'regexp.escape'],
				[/\\\./, 'regexp.invalid'],
				[/(\/)([dgimsuy]*)/, [{ token: 'regexp', bracket: '@close', next: '@pop' }, 'keyword.other']]
			],

			regexrange: [
				[/-/, 'regexp.escape.control'],
				[/\^/, 'regexp.invalid'],
				[/@regexpesc/, 'regexp.escape'],
				[/[^]]/, 'regexp'],
				[
					/]/,
					{
						token: 'regexp.escape.control',
						next: '@pop',
						bracket: '@close'
					}
				]
			],

			string_double: [
				[/[^\\"]+/, 'string'],
				[/@escapes/, 'string.escape'],
				[/\\./, 'string.escape.invalid'],
				[/"/, 'string', '@pop']
			],

			string_single: [
				[/[^\\']+/, 'string'],
				[/@escapes/, 'string.escape'],
				[/\\./, 'string.escape.invalid'],
				[/'/, 'string', '@pop']
			],

			string_backtick: [
				[/\$\{/, { token: 'delimiter.bracket', next: '@bracketCounting' }],
				[/[^\\`$]+/, 'string'],
				[/@escapes/, 'string.escape'],
				[/\\./, 'string.escape.invalid'],
				[/`/, 'string', '@pop']
			],

			bracketCounting: [
				[/\{/, 'delimiter.bracket', '@bracketCounting'],
				[/}/, 'delimiter.bracket', '@pop'],
				{ include: 'common' }
			]
		}
	});

	monaco.languages.registerCompletionItemProvider('badscript', {
		provideCompletionItems: () => {
			var suggestions = [
				{
					label: 'num',
					kind: monaco.languages.CompletionItemKind.Text,
					insertText: 'num'
				},
				{
					label: 'bool',
					kind: monaco.languages.CompletionItemKind.Text,
					insertText: 'bool'
				},
				{
					label: 'true',
					kind: monaco.languages.CompletionItemKind.Text,
					insertText: 'true'
				},
				{
					label: 'false',
					kind: monaco.languages.CompletionItemKind.Text,
					insertText: 'false'
				},
				{
					label: 'null',
					kind: monaco.languages.CompletionItemKind.Text,
					insertText: 'null'
				},
				{
					label: 'return',
					kind: monaco.languages.CompletionItemKind.Text,
					insertText: 'return'
				},
				{
					label: 'throw',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['throw "${1:message}";'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Throw Statement'
				},
				{
					label: 'if',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['if (${1:condition})', '{', '\t$0', '}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'If-Else Statement'
				},
				{
					label: 'try',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['try', '{', '\t$0', '}', 'catch(${1:ex})', '{', '\tthrow ${1:ex};', '}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Try-Catch Statement'
				},
				{
					label: 'else if',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['else if (${1:condition})', '{', '\t$0', '}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Else-If Statement'
				},
				{
					label: 'else',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['else', '{', '\t$0', '}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Else Statement'
				},
				{
					label: 'while',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['while (${1:condition})', '{', '\t$0', '}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'While Statement'
				},
				{
					label: 'foreach',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['foreach (${1:elem} in ${2:enumerable})', '{', '\t$0', '}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'For-Each Statement'
				},
				{
					label: 'for',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['for (let ${1:elem} = 0; ${1:elem} < ${2:size}; ${1:elem}++)', '{', '\t$0', '}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'For Statement'
				},
				{
					label: 'function',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['function ${1:func}(${2:args})','{', '\t$0', '}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Function Definition'
				},
				{
					label: 'class',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['class ${1:ClassName}','{', '\tfunction ${1:ClassName}()','\t{','\t\t$0', '\t}','}'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Class Definition'
				},
				{
					label: 'lambda-function',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['function(${1:args}) => $0'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Lambda Function Definition'
				},
				{
					label: 'export',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['Runtime.Export("${1:name}", {','\t$0', '});'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Export Statement'
				},
				{
					label: 'import',
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: ['Runtime.Import("${1:name}");'].join('\n'),
					insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
					documentation: 'Export Statement'
				},
			];
			return { suggestions: suggestions };
		}
	});
	
	CreateAutoComplete();
}

window.AddBadScriptLanguage = AddBadScriptLanguage;

