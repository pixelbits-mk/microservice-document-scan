using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Validation.Models
{
    public class ContentTypes
    {
        public static string[] All = new string[]
        {
            // Images
            // JPEG
            "image/jpeg",
            "image/pjpeg",  // Progressive JPEG

            // PNG
            "image/png",

            // GIF
            "image/gif",

            // BMP
            "image/bmp",
            "image/x-windows-bmp", // Alternate BMP MIME type

            // TIFF
            "image/tiff",
            "image/x-tiff",  // Alternate TIFF MIME type

            // WebP
            "image/webp",

            // SVG
            "image/svg+xml",

            // ICO
            "image/x-icon",
            // PDF
            "application/pdf",
            "application/x-pdf",   // Older, non-standard type
            "application/vnd.pdf", // Another variation

            // Microsoft Office Documents
            "application/msword",          // .doc
            "application/vnd.ms-excel",    // .xls
            "application/vnd.ms-powerpoint",// .ppt
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",  // .docx
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",        // .xlsx
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",// .pptx
            "application/vnd.openxmlformats-officedocument.wordprocessingml.template", // .dotx

            // Text Documents
            "text/plain",

            // Other Document Types
            "application/vnd.openxmlformats-officedocument.presentationml.slideshow",  // .ppsx
            "application/rtf",             // Rich Text Format
            "application/vnd.oasis.opendocument.text",    // .odt

            // Compressed Files
            "application/zip",
            "application/x-zip-compressed",
            "multipart/x-zip",

            // Other
            "application/octet-stream"
        };
    }
}


