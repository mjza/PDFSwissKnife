namespace PDFSwissKnife.Views;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class MainPage : ContentPage
{
	private List<string> mergeFiles = new List<string>();
	private string splitFile;
	private readonly Services.PdfLogic pdfLogic = new Services.PdfLogic();

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnSelectMergeFilesClicked(object sender, EventArgs e)
	{
		try
		{
			Console.WriteLine("OnSelectMergeFilesClicked invoked.");
			var result = await FilePicker.Default.PickMultipleAsync(new PickOptions
			{
				PickerTitle = "Select PDF files to merge",
				FileTypes = FilePickerFileType.Pdf
			});

			if (result != null)
			{
				mergeFiles = result.Select(file => file.FullPath).ToList();
				Console.WriteLine($"{mergeFiles.Count} files selected for merging.");
				await DisplayAlert("Files Selected", $"{mergeFiles.Count} files selected for merging.", "OK");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in OnSelectMergeFilesClicked: {ex.Message}");
			await DisplayAlert("Error", "An error occurred while selecting files.", "OK");
		}
	}

	private async void OnMergeFilesClicked(object sender, EventArgs e)
	{
		try
		{
			if (mergeFiles.Any())
			{
				var result = await FilePicker.Default.PickAsync(new PickOptions
				{
					PickerTitle = "Select the output PDF file",
					FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
					{
						{ DevicePlatform.MacCatalyst, new[] { "public.folder" } },
						{ DevicePlatform.WinUI, new[] { ".pdf" } }
					})
				});

				if (result != null)
				{
					pdfLogic.MergePdfs(mergeFiles, result.FullPath);
					await DisplayAlert("Success", "PDF files merged successfully.", "OK");
				}
			}
			else
			{
				await DisplayAlert("Error", "No files selected for merging.", "OK");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in OnMergeFilesClicked: {ex.Message}");
			await DisplayAlert("Error", "An error occurred while merging files.", "OK");
		}
	}

	private async void OnSelectSplitFileClicked(object sender, EventArgs e)
	{
		var result = await FilePicker.Default.PickAsync(new PickOptions
		{
			PickerTitle = "Select a PDF file to split",
			FileTypes = FilePickerFileType.Pdf
		});

		if (result != null)
		{
			splitFile = result.FullPath;
			await DisplayAlert("File Selected", $"File selected for splitting: {splitFile}", "OK");
		}
	}

	private async void OnSplitFileClicked(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(splitFile))
		{
			var result = await FilePicker.Default.PickAsync(new PickOptions
			{
				PickerTitle = "Select the output directory",
				FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
					{
						{ DevicePlatform.MacCatalyst, new[] { "public.folder" } },
						{ DevicePlatform.WinUI, new[] { ".pdf" } }
					})
			});

			if (result != null)
			{
				var outputDirectory = System.IO.Path.GetDirectoryName(result.FullPath);
				pdfLogic.SplitPdf(splitFile, outputDirectory);
				await DisplayAlert("Success", "PDF file split successfully.", "OK");
			}
		}
		else
		{
			await DisplayAlert("Error", "No file selected for splitting.", "OK");
		}
	}

}

