namespace PDFSwissKnife.Views;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public partial class MainPage : ContentPage
{
	private List<string> mergeFiles = new List<string>();
	private string? splitFile;
	private readonly Services.PdfLogic pdfLogic = new Services.PdfLogic();

	private readonly IFolderPicker folderPicker;

	public MainPage(IFolderPicker folderPicker)
	{
		InitializeComponent();
		this.folderPicker = folderPicker;
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
			Console.WriteLine("OnMergeFilesClicked invoked.");
			if (mergeFiles.Any())
			{
				var cancellationToken = new CancellationToken();
				var result = await folderPicker.PickAsync(cancellationToken);

				if (result.IsSuccessful)
				{
					var outputPath = System.IO.Path.Combine(result.Folder.Path, "Merged.pdf");

					// Check if the directory is writable
					if (!IsDirectoryWritable(result.Folder.Path))
					{
						await DisplayAlert("Error", "No write permission to the selected folder.", "OK");
						return;
					}

					pdfLogic.MergePdfs(mergeFiles, outputPath);
					Console.WriteLine("PDF files merged successfully.");
					await DisplayAlert("Success", "PDF files merged successfully. Saved as Merged.pdf", "OK");
				}
				else
				{
					await DisplayAlert("Error", $"The folder was not picked with error: {result.Exception.Message}", "OK");
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

	private bool IsDirectoryWritable(string path)
	{
		try
		{
			// Try to create a temporary file to check if the directory is writable
			string tempFile = System.IO.Path.Combine(path, System.IO.Path.GetRandomFileName());
			using (var fs = System.IO.File.Create(tempFile, 1, System.IO.FileOptions.DeleteOnClose))
			{
			}
			return true;
		}
		catch
		{
			return false;
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

