/**
 * @license Copyright (c) 2003-2021, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

//CKEDITOR.editorConfig = function( config ) {
//	// Define changes to default configuration here. For example:
//	// config.language = 'fr';
//	// config.uiColor = '#AADC6E';
//	config.extraPlugins = 'tokens';
//};

CKEDITOR.editorConfig = function (config) {
	config.toolbarGroups = [
		{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
		{ name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
		{ name: 'forms', groups: ['forms'] },
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
		{ name: 'links', groups: ['links'] },
		{ name: 'insert', groups: ['insert'] },
		{ name: 'styles', groups: ['styles'] },
		{ name: 'colors', groups: ['colors'] },
		{ name: 'tools', groups: ['tools'] },
		{ name: 'others', groups: ['others'] },
		{ name: 'about', groups: ['about'] }
	];

	config.removeButtons = 'Source,Sourcedialog,Save,Templates,Cut,Undo,Find,SelectAll,Scayt,Form,CopyFormatting,Anchor,Language,Flash,Table,HorizontalRule,SpecialChar,PageBreak,Iframe,BidiLtr,BidiRtl,JustifyLeft,Outdent,Indent,CreateDiv,Blockquote,JustifyCenter,JustifyRight,JustifyBlock,Strike,Subscript,Superscript,Checkbox,Radio,TextField,Textarea,Select,Button,ImageButton,HiddenField,Copy,Redo,Replace,NewPage,ExportPdf,Preview,Print,PasteFromWord,PasteText,Paste,Styles,Format,Font,FontSize,TextColor,BGColor,ShowBlocks,Maximize,About';
	/*config.extraPlugins = 'tokens';*/
};